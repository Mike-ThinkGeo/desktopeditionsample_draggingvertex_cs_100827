using System;
using System.Windows.Forms;
using System.Collections.ObjectModel;
using ThinkGeo.MapSuite.Core;
using ThinkGeo.MapSuite.DesktopEdition;


namespace  DraggingVertex
{
    public partial class TestForm : Form
    {
        public TestForm()
        {
            InitializeComponent();
        }

        private void TestForm_Load(object sender, EventArgs e)
        {
            winformsMap1.MapUnit = GeographyUnit.DecimalDegree;
            winformsMap1.CurrentExtent = new RectangleShape(-110.136,23.0523,-109.6883,22.7859);
            winformsMap1.BackgroundOverlay.BackgroundBrush = new GeoSolidBrush(GeoColor.FromArgb(255, 198, 255, 255));

            //Displays the World Map Kit as a background.
            ThinkGeo.MapSuite.DesktopEdition.WorldMapKitWmsDesktopOverlay worldMapKitDesktopOverlay = new ThinkGeo.MapSuite.DesktopEdition.WorldMapKitWmsDesktopOverlay();
            winformsMap1.Overlays.Add(worldMapKitDesktopOverlay);

            //Display the countries02 shapefile.
            ShapeFileFeatureLayer Layer1 = new ShapeFileFeatureLayer(@"../../data/countries02.shp");
            Layer1.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyles.CreateSimpleAreaStyle(GeoColor.StandardColors.Transparent, GeoColor.StandardColors.Black, 2);
            Layer1.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            LayerOverlay layerOverlay = new LayerOverlay();
            layerOverlay.Layers.Add(Layer1);
            winformsMap1.Overlays.Add(layerOverlay);

            //Gets the feature for Mexico for the EditInteractiveOverlay.
            Layer1.Open();
            Collection<Feature> features = Layer1.QueryTools.GetFeaturesByColumnValue("ISO_3DIGIT", "MEX");
            Layer1.Close();

            //EditInteractiveOverlay for doing editing such as resizing, rotating, dragging, adding vertex and moving vertex.
            EditInteractiveOverlay editOverlay = winformsMap1.EditOverlay;
            editOverlay.EditShapesLayer.Open();
            editOverlay.EditShapesLayer.EditTools.BeginTransaction();
            //Clears and add the Mexico feature to EditInteractiveOverlay
            editOverlay.EditShapesLayer.InternalFeatures.Clear();
            editOverlay.EditShapesLayer.EditTools.Add(features[0]);
            TransactionResult result = editOverlay.EditShapesLayer.EditTools.CommitTransaction();
            editOverlay.EditShapesLayer.BuildIndex();
            editOverlay.EditShapesLayer.Close();

            //Shows the control points for dragging, resizing and rotating the polygon feature as a whole.
            //Also shows the vertices to be dragged and add new vertex.
            editOverlay.CalculateAllControlPoints();
            //Builds the spatial index for better performance while doing any of those operations.
            editOverlay.ExistingControlPointsLayer.BuildIndex();

            winformsMap1.Refresh();
        }

      
        private void winformsMap1_MouseMove(object sender, MouseEventArgs e)
        {
            //Displays the X and Y in screen coordinates.
            statusStrip1.Items["toolStripStatusLabelScreen"].Text = "X:" + e.X + " Y:" + e.Y;

            //Gets the PointShape in world coordinates from screen coordinates.
            PointShape pointShape = ExtentHelper.ToWorldCoordinate(winformsMap1.CurrentExtent, new ScreenPointF(e.X, e.Y), winformsMap1.Width, winformsMap1.Height);

            //Displays world coordinates.
            statusStrip1.Items["toolStripStatusLabelWorld"].Text = "(world) X:" + Math.Round(pointShape.X, 4) + " Y:" + Math.Round(pointShape.Y, 4);
        }
        
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}
