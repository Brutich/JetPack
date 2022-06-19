using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.DesignScript.Runtime;
using Revit.Elements;
using RevitServices.Persistence;
using Dynamo.Graph.Nodes;


namespace Utilities
{

    /// <summary>
    /// The Room class.
    /// </summary>
    public class Room
    {
        private Room() { }

        private static readonly Document document = DocumentManager.Instance.CurrentDBDocument;

        /// <summary>
        /// </summary>
        /// <param name="room"></param>
        /// <returns></returns>
        /// <search>
        /// rooms, from, to, doors
        /// </search> 
        [IsVisibleInDynamoLibrary(true)]
        [NodeCategory("Query")]
        [MultiReturn("FromDoors", "ToDoors")]
        public static Dictionary<string, object> Doors(Revit.Elements.Room room)
        {
            if (room == null) return null;

            Document document = DocumentManager.Instance.CurrentDBDocument;

            var fec = new FilteredElementCollector(document);
            fec.OfCategory(BuiltInCategory.OST_Doors);
            fec.OfClass(typeof(Autodesk.Revit.DB.FamilyInstance));
            var elements = fec.ToElements();

            if (!elements.Any())
                return new Dictionary<string, object>
                {
                    { "FromDoors", Enumerable.Empty<Revit.Elements.FamilyInstance>() },
                    { "ToDoors", Enumerable.Empty<Revit.Elements.FamilyInstance>() }
                };

            var fromDoors = new List<Revit.Elements.FamilyInstance>();
            var toDoors = new List<Revit.Elements.FamilyInstance>();
            var rm = room.InternalElement as Autodesk.Revit.DB.Architecture.Room;
            foreach (var element in elements)
            {
                var door = element as Autodesk.Revit.DB.FamilyInstance;

                if (door.ToRoom?.Id == rm.Id)
                {
                    toDoors.Add(element.ToDSType(true) as Revit.Elements.FamilyInstance);
                    continue;
                }

                if (door.FromRoom?.Id == rm.Id)
                {
                    fromDoors.Add(element.ToDSType(true) as Revit.Elements.FamilyInstance);
                }
            }

            return new Dictionary<string, object>
            {
                { "FromDoors", fromDoors},
                { "ToDoors", toDoors}
            }; 
        }
    }
}
    