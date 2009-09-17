using Gtk;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Reflection;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace MyInventory.GtkGui {
	public abstract class ListNotifyModel<T>
	: EnumerableNotifyModel<T>, TreeDragSourceImplementor, TreeDragDestImplementor
	where T : class, IEnumerable, IList
	{
		public ListNotifyModel(T col) 
		: base(col)
		{}
		
		private T GetPathCollection(TreePath path) {
			// we can't simply write
			// NodeGetPathNode(path).Collection
			// since the inserted item might be the first one and if that was true,
			// the NodeGetPathNode would return null and there would be no way to
			// get to the collection
			T enu;
			TreePath p = path.Copy();
			
			p.Up();
			if(p.Depth != 0){
				Node node = NodeGetPathNode(p);
				if(node == null) return null;
				enu = GetCollection(node.NodeVal);
			}
			else {
				enu = Collection;
			}
			return enu;
		}
		
		public bool DragDataReceived(TreePath destPath, SelectionData data)
		{
			TreeModel srcModel;
			TreePath srcPath;
			if(Tree.GetRowDragData(data, out srcModel, out srcPath)){
				if(IsSameTreeModel(srcModel)){
					T destList = GetPathCollection(destPath);
					if(destList != null){
						Node srcNode = NodeGetPathNode(srcPath);
						if(srcNode != null){
							int destPos = destPath.Indices[destPath.Depth-1];
							// now copy the node to the target				
							try {
								object nodeVal = srcNode.NodeVal;
								destList.Insert(destPos,nodeVal);
							}
							catch(ArgumentOutOfRangeException) {
								return false;
							}
							
							return true;
						}
					}
				}
			}
			return false;
		}
		
		private bool IsSameTreeModel(TreeModel srcModel){
			// we need to check if srcModel and this are the same which isn't that easy
			// since srcModel was created with TreeModelAdapter
			// so we compare the stamps of the models by getting an iter from src
			// it should have one for sure since s.th was dragged from it
			TreeIter someSrcIter;
			return srcModel.GetIterFirst(out someSrcIter) && someSrcIter.Stamp == Stamp;
		}
		
		public bool RowDropPossible(TreePath destPath, SelectionData data)
		{
			TreeModel srcModel;
			TreePath srcPath;
			if(Tree.GetRowDragData(data, out srcModel, out srcPath)){
				if(IsSameTreeModel(srcModel)){
					// can't drop me in one of my sub items
					if(srcPath.IsAncestor(destPath))
						return false;
					
					// dropping me right before or after me doesn't change s.th
					/*int srcPos = srcPath.Indices[srcPath.Depth-1];
					int destPos = destPath.Indices[destPath.Depth-1];
					srcPath.Up();
					destPath.Up();
					if(srcPath == destPath && (srcPos == destPos+1 ||
					                           srcPos == destPos ||
					                           srcPos == destPos -1 ))
						return false;*/
					
					return true;
				}
			}
			
			return false;
		}
		
		public bool DragDataDelete(TreePath path) {
			int pos = path.Indices[path.Depth-1];
			T list = GetPathCollection(path);
			
			if(list != null){
				try {
					list.RemoveAt(pos);
					return true;
				}
				catch(ArgumentOutOfRangeException)
				{}
			}
			return false;
		}
		
		public bool DragDataGet(TreePath path, SelectionData data) {
			return Tree.SetRowDragData(data,new TreeModelAdapter(this),path);
		}
		
		public bool RowDraggable(TreePath path) {
			return true;
		}
	}
}
