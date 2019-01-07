using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using System.Collections;
using Autodesk.Revit;
using RevitServices.Persistence;
using Revit.Elements;

namespace JetPack_Z
{
    class FamilyInstance2
    {
        private FamilyInstance2() { }

        public static bool IsFlipped(Revit.Elements.FamilyInstance familyInstance)
        {
            // Unwrap element
            Autodesk.Revit.DB.FamilyInstance element = (Autodesk.Revit.DB.FamilyInstance)familyInstance.InternalElement;

            bool handFlipped = element.HandFlipped;
            bool facingFlipped = element.FacingFlipped;

            return handFlipped ^ facingFlipped;
        }
    }
}
