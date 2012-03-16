using System.Diagnostics;
using System.IO;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace AMEE_in_Revit.Addin.CO2eParameter
{
    public class SharedParameterManipulator
    {
        private readonly Application _dbApplication;
        private readonly UIApplication _application;
        private readonly Document _activeUIDocument;

        public SharedParameterManipulator(UIApplication application)
        {
            _dbApplication = application.Application;
            _application = application;
            _activeUIDocument = _application.ActiveUIDocument.Document;
        }

        public void ApplyParameterToAllElements(SharedParameterDefinition co2EParam)
        {
            var transaction = new Transaction(_activeUIDocument, "ApplyParameterToAllBuiltInCategories");
            transaction.Start();

            var definition = OpenDefinition(co2EParam.ParameterName, co2EParam.ParameterType, 
                                OpenGroup(co2EParam.GroupName, 
                                    OpenSharedParamFile(co2EParam.SharedParametersFilename)));
            
            AttachCategoryToAllBuildInCategories(definition);

            transaction.Commit();
        }

        private void AttachCategoryToAllBuildInCategories(Definition definition)
        {
            foreach (var builtInCategoryId in Settings.BuiltInCategoriesWithCO2eParameter)
            {
                var category = _activeUIDocument.Settings.Categories.get_Item(builtInCategoryId);
                var categorySet = _dbApplication.Create.NewCategorySet();
                categorySet.Insert(category);

                var instanceBinding = _dbApplication.Create.NewInstanceBinding(categorySet);
                _activeUIDocument.ParameterBindings.Insert(definition, instanceBinding, BuiltInParameterGroup.PG_ENERGY_ANALYSIS);

                Debug.WriteLine("Added custom shared param {0} to category {1}", definition.Name, category.Name);
            }
        }

        private Definition OpenDefinition(string parameterName, ParameterType parameterType, DefinitionGroup definitionGroup)
        {
            var definition = definitionGroup.Definitions.get_Item(parameterName)
                                 ?? definitionGroup.Definitions.Create(parameterName, parameterType, true);
            return definition;
        }

        private DefinitionGroup OpenGroup(string groupName, DefinitionFile definitionFile)
        {
            return definitionFile.Groups.get_Item(groupName) 
                ?? definitionFile.Groups.Create(groupName);
        }

        private DefinitionFile OpenSharedParamFile(string sharedParametersFilename)
        {
            var fullPath = Path.GetFullPath(sharedParametersFilename);
            if (!File.Exists(fullPath)) File.WriteAllText(fullPath, "");
            _dbApplication.SharedParametersFilename = fullPath;
            if (_dbApplication.OpenSharedParameterFile()==null)
            {
                throw new OpenSharedParamFileException(
                    string.Format("Unable to create / open shared parameter file: {0}", fullPath));
            }
            return _dbApplication.OpenSharedParameterFile();
        }
    }
}
