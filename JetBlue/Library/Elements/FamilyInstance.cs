using Autodesk.DesignScript.Runtime;
using Revit.Elements;
using RevitServices.Persistence;
using RevitServices.Transactions;
using Dynamo.Graph.Nodes;


namespace Elements
{
    /// <summary>
    /// The FamilyInstance class.
    /// </summary>
    public class FamilyInstance
    {

        private FamilyInstance() { }

        /// <summary>
        /// The node returns "true" if family instance is mirrored. (only one axis is flipped)
        /// </summary>
        /// <param name="familyInstance">Family instance for flipping test.</param>
        /// <returns>Is flipped</returns>
        /// <search>
        /// family, instance, test, flipped, mirrored
        /// </search> 
        [IsVisibleInDynamoLibrary(true)]
        [NodeCategory("Query")]
        public static bool IsMirrored(Revit.Elements.FamilyInstance familyInstance)
        {
            // Unwrap element
            var instance = (Autodesk.Revit.DB.FamilyInstance)familyInstance.InternalElement;

            return instance.Mirrored;
        }


        /// <summary>
        /// The "To Room" set for the door or window in the last phase of the project.
        /// </summary>
        /// <param name="familyInstance">Door or window family instance.</param>
        /// <returns>Room</returns>
        /// <search>
        /// to room, instance, door, window
        /// </search> 
        [IsVisibleInDynamoLibrary(true)]
        [NodeCategory("Query")]
        public static Revit.Elements.Room ToRoom(Revit.Elements.FamilyInstance familyInstance)
        {
            if (familyInstance == null) return null;

            var instance = (Autodesk.Revit.DB.FamilyInstance)familyInstance.InternalElement;
            var toRoom = instance.ToRoom;

            return toRoom is null ? null : toRoom.ToDSType(true) as Revit.Elements.Room;
        }


        /// <summary>
        /// The "From Room" set for the door or window in the last phase of the project.
        /// </summary>
        /// <param name="familyInstance">Door or window family instance.</param>
        /// <returns>Room</returns>
        /// <search>
        /// from room, instance, door, window
        /// </search> 
        [IsVisibleInDynamoLibrary(true)]
        [NodeCategory("Query")]
        public static Revit.Elements.Room FromRoom(Revit.Elements.FamilyInstance familyInstance)
        {
            if (familyInstance is null)
                return null;

            var instance = (Autodesk.Revit.DB.FamilyInstance)familyInstance.InternalElement;
            var fromRoom = instance.FromRoom;

            return fromRoom is null ? null : fromRoom.ToDSType(true) as Revit.Elements.Room;
        }


        /// <summary>
        /// Changes the type of family instance to another.
        /// </summary>
        /// <param name="familyInstance">Family instance for changing type.</param>
        /// <param name="familyType">Another family type.</param>
        /// <returns></returns>
        /// <search>
        /// change, type, family, instance
        /// </search>
        [IsVisibleInDynamoLibrary(true)]
        public static Revit.Elements.FamilyInstance ChangeType(
            Revit.Elements.FamilyInstance familyInstance,
            Revit.Elements.FamilyType familyType)
        {
            // Unwrap input parameters
            var instance = familyInstance.InternalElement as Autodesk.Revit.DB.FamilyInstance;
            Autodesk.Revit.DB.Element anotherType = familyType.InternalElement;

            TransactionManager.Instance.EnsureInTransaction(DocumentManager.Instance.CurrentDBDocument);
            instance.ChangeTypeId(anotherType.Id);
            TransactionManager.Instance.TransactionTaskDone();

            return familyInstance;
        }


        /// <summary>
        /// Node to get the super component of current family instance.
        /// </summary>
        /// <param name="familyInstance">Family instance for getting the super component.</param>
        /// <returns></returns>
        /// <search>
        /// super component, family, instance
        /// </search>
        [IsVisibleInDynamoLibrary(true)]
        [NodeCategory("Query")]
        public static Revit.Elements.FamilyInstance SuperComponent(Revit.Elements.FamilyInstance familyInstance)
        {
            // Unwrap input parameters
            var instance = familyInstance.InternalElement as Autodesk.Revit.DB.FamilyInstance;
            var superComponent = instance.SuperComponent as Autodesk.Revit.DB.FamilyInstance;

            return superComponent is null ? null : superComponent.ToDSType(true) as Revit.Elements.FamilyInstance;
        }
    }
}
    