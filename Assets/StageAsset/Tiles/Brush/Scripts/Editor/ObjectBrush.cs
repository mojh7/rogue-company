using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace UnityEditor
{
    [CustomGridBrush(true, false, false, "Object Brush")]
    public class ObjectBrush : GridBrush
    {
        public int z = 0;

        public override void Paint(GridLayout grid, GameObject brushTarget, Vector3Int position)
        {
            // Do not allow editing palettes
            if (brushTarget.layer == 31)
                return;
            Tilemap tilemap = brushTarget.GetComponent<Tilemap>();
            TileBase tilebase = tilemap.GetTile(position);
            ObjectTile objectTile = tilebase as ObjectTile;

            if (objectTile == null)
            {
                base.Paint(grid, brushTarget, position);

                objectTile = tilemap.GetTile(position) as ObjectTile;
                if(objectTile == null)
                {
                    return;
                }
                else
                {
                    tilemap.SetTile(position, tilebase);
                    GameObject prefab = objectTile.GetGameObject();
                    GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
                    Undo.RegisterCreatedObjectUndo((Object)instance, "Paint Prefabs");
                    if (instance != null)
                    {
                        instance.transform.SetParent(brushTarget.transform);
                        instance.transform.position = grid.LocalToWorld(grid.CellToLocalInterpolated(new Vector3Int(position.x, position.y, - 1) + new Vector3(.5f, .5f, 0f)));
                        instance.transform.parent = brushTarget.transform;
                    }
                }
            }
          
        }

        public override void Erase(GridLayout grid, GameObject brushTarget, Vector3Int position)
        {
            // Do not allow editing palettes
            if (brushTarget.layer == 31)
                return;

            Transform erased = GetObjectInCell(grid, brushTarget.transform, new Vector3Int(position.x, position.y, -1));
            if (erased != null)
                Undo.DestroyObjectImmediate(erased.gameObject);
            else
                base.Erase(grid, brushTarget, position);
        }

        private static Transform GetObjectInCell(GridLayout grid, Transform parent, Vector3Int position)
        {
            int childCount = parent.childCount;
            Vector3 min = grid.LocalToWorld(grid.CellToLocalInterpolated(position));
            Vector3 max = grid.LocalToWorld(grid.CellToLocalInterpolated(position + Vector3Int.one));
            Bounds bounds = new Bounds((max + min) * .5f, max - min);

            for (int i = 0; i < childCount; i++)
            {
                Transform child = parent.GetChild(i);
                if (bounds.Contains(child.position))
                    return child;
            }
            return null;
        }

        [MenuItem("Assets/Create/Coordinate Brush")]
        public static void CreateBrush()
        {
            string path = EditorUtility.SaveFilePanelInProject("Save Coordinate Brush", "New Coordinate Brush", "asset", "Save Coordinate Brush", "Assets");

            if (path == "")
                return;

            AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<CoordinateBrush>(), path);
        }
    }

    [CustomEditor(typeof(ObjectBrush))]
    public class ObjectBrushEditor : GridBrushEditor
    {
        private ObjectBrush objectBrush { get { return target as ObjectBrush; } }

        public override void PaintPreview(GridLayout grid, GameObject brushTarget, Vector3Int position)
        {
            var zPosition = new Vector3Int(position.x, position.y, objectBrush.z);
            base.PaintPreview(grid, brushTarget, zPosition);
        }

        public override void OnPaintSceneGUI(GridLayout grid, GameObject brushTarget, BoundsInt position, GridBrushBase.Tool tool, bool executing)
        {
            base.OnPaintSceneGUI(grid, brushTarget, position, tool, executing);
        }
    }
}
