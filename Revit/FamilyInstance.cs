namespace JetPack_Z
{
    /// <summary>
    /// The FamilyInstance2 class.
    /// </summary>
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
