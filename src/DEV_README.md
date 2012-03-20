#DEV Readme

##Developer machine setup

To build the AMEE-in-Revit addin you will need:

 * Autodesk Revit 2012
 * Autodesk Revit Addin Manager (recommended)
 * Visual Studio 2010 C# Express (Visual Studio 2010 Pro+ recommended)

Alterately, Amazon EC2 AMI ami-d1774fa5 contains all the required software to build and test the AMEE-in-Revit addin.

##Architecture

<img src="http://dl.dropbox.com/u/33609233/AMEE/screenshots/architecture.jpeg">

###AMEEConnect
AMEEConnect is a REST based API exposed over HTTP.  

###AMEEClient
The AMEEClient library is a .NET based REST client library, with a series of convenience methods 
for mapping a material name to the relevant CO2e data stored in AMEE.

The following code snippet illustrates how the AMEEClient library would be used to 
calculate the CO2e value for 10kg of copper

    var _ameeClient = new Client(new Uri(AmeeUrl), AmeeUserName, AmeePassword);
    var mapper = new AMEEClient.MaterialMapper.MaterialMapper(@"MaterialMapper\SampleMaterialMap.xml", _ameeClient);
	var co2eCopper = mapper.GetMaterialDataItem("1:Copper").CalculateCO2eByMass(10);

###AMEE-in-Revit.Addin
The AMEE-in-Revit Addin is a .NET4 based plugin for the Revit 2012 Architecture BIM tool 
that connects the materials in the BIM model to the AMEEConnect data for 
those materials via the AMEEClient library.

The AMEE-in-Revit Addin contains the following "modules":

1.  Ribbon integration - AMEE_in_Revit_Initialise implements IExternalApplication, which in run on Revit application startup.  
This class adds a number of buttons to the Analyse tab in the Revit ribbon.
1.  CO2eParameter - a Revit SharedParameter named CO2e is created (currently manually) for every element in the building model.  
An IUpdater named CO2eFieldUpdater is registered to be triggered on every model element change.  When triggered, the CO2eFieldUpdater
queries the element for its volume, passes this to the AMEEClient and stores the resulting CO2e value in the CO2e shared parameter 
for the element.
1.  Visualizations - A visualization of the CO2e parameters for every element is attached to the CO2e 3d view in the model.  This view 
attaches the CO2e value of each element as a measurement to each corner of each face of the element in the view.  A custom AnalysisDisplayStyle
is attached to the view that colours the face measurements according to a heatmap style spectrum of colours, ranging from green (low)
through to red (high).
1.  Commands - are a series of classes implementing the IExternalCommand interface, triggered by the Ribbon buttons or the developer
focussed Add-in Manager tool.  Typically these perform batch operations, such as calculating the CO2e value for all elements in the model.
1.  Logging - Log4Net is used to write runtime log files to the debug console & a text based log file (C:\AMEE-in-Revit.Addin.log by default)

###Further documentation

1.  samples\Revit 2012 SDK contains useful documentation, tools & sample code
1.  http://thebuildingcoder.typepad.com is a very useful selection of Revit Addin tips & techniques.  Check here first when wondering 
how to do something in the Revit API
1.  A useful technique for debugging without having to constantly start & stop Revit is described at:
