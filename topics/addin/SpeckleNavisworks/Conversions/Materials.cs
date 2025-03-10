﻿using Autodesk.Navisworks.Api;
using Color = System.Drawing.Color;

namespace SpeckleNavisworks.Conversions {
  class Materials {
    static public Objects.Other.RenderMaterial TranslateMaterial ( ModelItem geom ) {

      string materialName;

      var Settings = new { Mode = "original" };

      Color renderColor;

      switch ( Settings.Mode ) {
        case "original":
          renderColor = Colors.NavisColorToColor( geom.Geometry.OriginalColor );
          break;
        case "active":
          renderColor = Colors.NavisColorToColor( geom.Geometry.ActiveColor );
          break;
        case "permanent":
          renderColor = Colors.NavisColorToColor( geom.Geometry.PermanentColor );
          break;
        default:
          renderColor = new Color();
          break;
      }

      materialName = $"NavisMaterial_{Math.Abs( renderColor.ToArgb() )}";

      Color black = Color.FromArgb( Convert.ToInt32( 0 ), Convert.ToInt32( 0 ), Convert.ToInt32( 0 ) );

      PropertyCategory itemCategory = geom.PropertyCategories.FindCategoryByDisplayName( "Item" );
      if ( itemCategory != null ) {
        DataPropertyCollection itemProperties = itemCategory.Properties;
        DataProperty itemMaterial = itemProperties.FindPropertyByDisplayName( "Material" );
        if ( itemMaterial != null && itemMaterial.DisplayName != "" ) {
          materialName = itemMaterial.Value.ToDisplayString();
        }
      }

      PropertyCategory materialPropertyCategory = geom.PropertyCategories.FindCategoryByDisplayName( "Material" );
      if ( materialPropertyCategory != null ) {
        DataPropertyCollection material = materialPropertyCategory.Properties;
        DataProperty name = material.FindPropertyByDisplayName( "Name" );
        if ( name != null && name.DisplayName != "" ) {
          materialName = name.Value.ToDisplayString();
        };
      }

      Objects.Other.RenderMaterial r = new Objects.Other.RenderMaterial( 1 - geom.Geometry.OriginalTransparency, 0, 1, renderColor, black ) {
        name = materialName
      };

      return r;
    }
  }
}
