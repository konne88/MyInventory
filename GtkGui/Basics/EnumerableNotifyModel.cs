/* MyInventory - Keep track of your private assets. 
 * Copyright (C) 2008-2010 Konstantin Weitz
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 3
 * of the License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301, USA.
 */

// An example TreeModel implementation
// http://www.mail-archive.com/gtk-sharp-list@lists.ximian.com/msg02737.html

using Gtk;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Reflection;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace MyInventory.GtkGui {
	public abstract class EnumerableNotifyModel<T>
	: GLib.Object, TreeModelImplementor
	where T : class, IEnumerable
	{
		//-------------------------- 
		// Actual Model starts here
		//--------------------------
		public EnumerableNotifyModel(T col)
		{
			GCHandle gch = GCHandle.Alloc(this);
			Stamp = ((IntPtr)gch).ToInt32();
			
			Collection = col;
			
			// connect the model's events
			ConnectCollection(Collection);
			Node node = Node.CreateFirstModelNode(Collection);
			ConnectNodes(node);
		}
		
		private void ConnectNodes(Node node)
		{
			if(node != null) {
				do {
					ConnectNode(node);
					
					Node subNode = NodeGetFirstChild(node);
					ConnectNodes(subNode);
				} while(NodeMoveNext(node));
			}
		}
	    
		protected abstract T GetCollection(object nodeVal);
		protected abstract object GetObject(object nodeVal);
		protected abstract object GetColumnValue(object o, int col);
		protected abstract Type ProvideColumnType(int col);
		public abstract int NColumns { 
			get;
		}
		
		public virtual TreeModelFlags Flags { 
			get {
				return 0;
			}
		}
		
		public GLib.GType GetColumnType(int col){
			return (GLib.GType)ProvideColumnType(col);
		}
		
		private void ErrorOut(string o){
			//Console.WriteLine(o);
		}
		
		private Node GetIterNode(TreeIter iter) {
			GCHandle gch = (GCHandle)iter.UserData;
			return (Node)gch.Target;
		}
	
		private TreeIter CreateIterFromNode(Node node)
		{
			TreeIter iter = TreeIter.Zero;
			iter.Stamp = Stamp;
			GCHandle gch = GCHandle.Alloc (node);
			iter.UserData = (IntPtr)gch;
			return iter;
		}
		
		private bool IsIterUseable(TreeIter iter){
			if(iter.UserData == IntPtr.Zero){
				ErrorOut("ITER IS ZERO");
				return false;
			}
			if(iter.Stamp != Stamp){
				Console.WriteLine("INVALID ITER STAMP");				
				Console.WriteLine("View Stamp: "+Stamp.ToString());
				Console.WriteLine("Iter Stamp: "+iter.Stamp.ToString());
				return false;
			}
			return true;
		}
		
		public bool IterIsValid(TreeIter iter){
			ErrorOut("ITERISVALID");
			return iter.Stamp == Stamp;
		}
		
	    public void GetValue(TreeIter iter, int col, ref GLib.Value val) {
			ErrorOut("GETVALUE");
			ErrorOut(col.ToString());
			if(IsIterUseable(iter)){
				val = new GLib.Value( GetColumnValue(GetObject(GetIterNode(iter).NodeVal),col) );
			}
		}
	
	    public bool GetIter(out TreeIter iter, TreePath path) {
			ErrorOut("GETITER ");
			ErrorOut(path.ToString());
			
			Node node = NodeGetPathNode(path);
			if(node != null){
				iter = CreateIterFromNode(node);
				return true;
			}
			
			iter = TreeIter.Zero;
			return false;
	    }
	
	    public TreePath GetPath(TreeIter iter){
			ErrorOut("GETPATH");
			if(IsIterUseable(iter)){
				return GetIterNode(iter).Path;
			}
			return TreePath.NewFirst();
	    }
	
		public bool IterChildren(out TreeIter child, TreeIter iter) {
			ErrorOut("GETCHILDREN");
	    	
	    	if(IsIterUseable(iter)){
				Node node = NodeGetFirstChild(GetIterNode(iter));
				if(node != null){
					child = CreateIterFromNode(node);
		            return true;
				}
			}
			
			child = TreeIter.Zero;
			return false;
		}
	
	    public bool IterHasChild(TreeIter iter) {
			ErrorOut("HASCHILDREN");
			
			if(IsIterUseable(iter))
				return NodeHasChild(GetIterNode(iter));
			
			return false;
	    }
	
	    public int IterNChildren(TreeIter iter) {
			ErrorOut("ITERNCHILDREN");
			
			Node node;
				
			// we count the root notes
			if(iter.UserData == IntPtr.Zero){
				node = Node.CreateFirstModelNode(Collection);
			}
			// we count the iters children
			else if(IsIterUseable(iter)){
				node = NodeGetFirstChild(GetIterNode(iter));
			}
			else
				return 0;
			if(node == null)
				return 0;

			return NodeCountSiblings(node);
	    }
	
	    public bool IterNext(ref TreeIter iter) {
			ErrorOut("NEXT");
			
	    	if(IsIterUseable(iter))
				return NodeMoveNext(GetIterNode(iter));
			
			return false;
	    }
	
	    public bool IterNthChild(out TreeIter child, TreeIter iter, int pos) {		
			ErrorOut("NTHCHILD");
			ErrorOut(pos.ToString());
			
			if(IsIterUseable(iter)){
				Node node = NodeGetFirstChild(GetIterNode(iter));
				if(node != null){
					if(NodeMoveSteps(node,pos)){
						child = CreateIterFromNode(node);
						return true;
					}
				}
			}
			
			child = TreeIter.Zero;
			return false;
	    }
	
	    public bool IterParent(out TreeIter parent, TreeIter iter) {
			ErrorOut("ITERPARENT");
	    				
	    	if(IsIterUseable(iter)){
				ErrorOut(GetIterNode(iter).Path.ToString());
				Node node = NodeGetParent(GetIterNode(iter));
				
				if(node != null){
					parent = CreateIterFromNode(node);
					
					ErrorOut("TRUE");
					ErrorOut(node.Path.ToString());
					
					return true;
				}
			}
			
			parent = TreeIter.Zero;
			return false;
	    }
		
		public void RefNode(TreeIter iter)
		{}
		
		public void UnrefNode(TreeIter iter)
		{}
		
		public readonly int Stamp;
		
	    public readonly T Collection;
		
		//-------------------------------
		// Event Handling implementation
		//-------------------------------
	    private TreePath FindFirstChildPathOfCollection(T col)
	    {
			TreePath found = null;
	    	if(object.ReferenceEquals(Collection, col))
			    found = TreePath.NewFirst();
			else {
				TreeModel model = new TreeModelAdapter(this);
				model.Foreach(delegate(TreeModel m, TreePath p, TreeIter i){
					if(object.ReferenceEquals(GetCollection(GetIterNode(i).NodeVal), col)){
				    	TreePath path = p.Copy();
						path.Down();
						found = path;
						return true;
					}
					return false;
				});
			}
			
			return found;
	    }
	    		
		private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs args) {
			TreeModel m = new TreeModelAdapter(this);
			
			switch(args.Action){
			case NotifyCollectionChangedAction.Add: 
			{
				TreePath path = FindFirstChildPathOfCollection((T)sender);
				
				Node n = NodeGetPathNode(path);
				NodeMoveSteps(n,args.NewStartingIndex);
				
				foreach(object nodeVal in args.NewItems){
					ConnectObject(GetObject(nodeVal));
					ConnectCollection(GetCollection(nodeVal));
					m.EmitRowInserted(n.Path,CreateIterFromNode(n));
					m.EmitRowHasChildToggled(n.Path,CreateIterFromNode(n));
					NodeMoveNext(n);
				}
				break;
			}
			case NotifyCollectionChangedAction.Remove:
			{
				TreePath path = FindFirstChildPathOfCollection((T)sender);
				
				int[] ind = path.Indices;
				ind[path.Depth-1] = args.OldStartingIndex;
				TreePath p = new TreePath(ind);
				
				foreach(object nodeVal in args.OldItems){
					DisconnectObject(GetObject(nodeVal));
					DisconnectCollection(GetCollection(nodeVal));
					m.EmitRowDeleted(p);
					p.Next();
				}				
				break;
			}
			case NotifyCollectionChangedAction.Replace:
				//int[] newOrder = {};
				//m.EmitRowsReordered(n.Path,i,newOrder);
					
				break;
			case NotifyCollectionChangedAction.Move:
				//m.EmitRowsReordered();
				
				break;
			case NotifyCollectionChangedAction.Reset:
				//m.EmitRowsReordered();
				
				break;
			}
		}
		
	    private Node FindNodeOfObject(object o)
	    {
	    	Node found = null;
	    	TreeModel model = new TreeModelAdapter(this);
			model.Foreach(delegate (TreeModel m, TreePath p, TreeIter i){
				Node node = GetIterNode(i);
				
				if(object.ReferenceEquals(GetObject(node.NodeVal), o)){
					found = NodeCopy(node);
					return true;
				}
				return false;
			});
			
			return found;
	    }
		
		private void OnObjectChanged(object sender, PropertyChangedEventArgs e){
			TreeModel m = new TreeModelAdapter( this );
			
			Node n = FindNodeOfObject(sender);
			m.EmitRowChanged(n.Path,CreateIterFromNode(n));
		}
		
		private void ConnectNode(Node node){
			object nodeVal = node.NodeVal;
			ConnectObject(GetObject(nodeVal));
			ConnectCollection(GetCollection(nodeVal));
		}

		private void DisconnectNode(Node node){
			// this is not used yet (and may be never used) because
			// often it is just done by calling DisconnectObject etc
			// directly
			object nodeVal = node.NodeVal;
			DisconnectObject(GetObject(nodeVal));
			DisconnectCollection(GetCollection(nodeVal));
		}
		
		private void ConnectCollection(T col){
			INotifyCollectionChanged notif = col as INotifyCollectionChanged;
			if(notif != null)
				notif.CollectionChanged += OnCollectionChanged;
		}
		
		private void DisconnectCollection(T col){
			INotifyCollectionChanged notif = col as INotifyCollectionChanged;
			if(notif != null)
				notif.CollectionChanged -= OnCollectionChanged;
		}
			
		private void ConnectObject(object o){
			INotifyPropertyChanged notif = o as INotifyPropertyChanged;
			if(notif != null)
				notif.PropertyChanged += OnObjectChanged;
		}
		
		private void DisconnectObject(object o){
			INotifyPropertyChanged notif = o as INotifyPropertyChanged;
			if(notif != null)
				notif.PropertyChanged -= OnObjectChanged;
		}
		
		//----------------------------
		// This is all about the Node
		//----------------------------
		protected class Node {
			public Node()
			{}
		
			public static Node CreateFirstModelNode(T model){
				Node n = new Node();
				n.Collection = model;
				n.Path = TreePath.NewFirst();
				n.Position = model.GetEnumerator();
				if(!n.Position.MoveNext())
					return null;
				return n;
			}
			
			public static Node CreateFirstChildNode(T collection, TreePath pathToParent){
				if(collection == null)
					return null;
				IEnumerator position = collection.GetEnumerator();
				if(!position.MoveNext())
					return null;
				TreePath path = pathToParent.Copy();
				path.Down();
				
				Node n = new Node();
				n.Collection = collection;
				n.Position = position;
				n.Path = path;
				return n;
			}
			
			public static Node CreateParentNode(T collection,IEnumerator position,TreePath pathToChild){
				TreePath path = pathToChild.Copy();
				path.Up();
				if(path.Depth == 0)
					return null;
				
				Node n = new Node();
				n.Position = position;
				n.Collection = collection;
				n.Path = path;
				return n;
			}
			
			public object NodeVal {
				get {
					return Position.Current;
				}				
			}
			
			public IEnumerator Position;
			public T Collection;
			public TreePath Path;
		}
	
		protected virtual Node NodeCopy(Node n){
			Node nn = new Node();
			nn.Collection = n.Collection;
			// setup the parents path
			int[] ind = n.Path.Indices;
			ind[n.Path.Depth-1] = 0;
			nn.Path = new TreePath(ind);
			nn.Position = n.Collection.GetEnumerator();
			// setup the position
			nn.Position.MoveNext();
			NodeMoveSteps(nn,n.Path.Indices[n.Path.Depth-1]);
			return nn;
		}
	
		protected virtual bool NodeMoveNext(Node node){
			if(!node.Position.MoveNext()){
				return false;
			}
			node.Path.Next();
			return true;
		}
		
		protected virtual bool NodeMoveSteps(Node node, int steps){
			for(int i=0 ; i<steps ; ++i)
				if(NodeMoveNext(node) == false)
					return false;
			
			return true;
		}
	
		protected virtual Node NodeGetParent(Node node){
			TreePath path = node.Path;
			path.Up();
			return NodeGetPathNode(path);
		}
		
		protected virtual Node NodeGetPathNode(TreePath path){
			int[] ind = path.Indices;
			if(ind.Length == 0)
				return null;
			Node node = Node.CreateFirstModelNode(Collection);
			
			for(int i=0; i<ind.Length ; ++i){
				// we don't get the child the very first time
				if(i!=0){
					node = NodeGetFirstChild(node);
				}
				if(node == null)
					return null;
				if(NodeMoveSteps(node,ind[i]) == false)
					return null;
			}
			
			return node;
		}
		
		protected virtual int NodeCountSiblings(Node node){
			IEnumerator e = node.Collection.GetEnumerator();
			int c = 0;
			while(e.MoveNext())
				++c;
			return c;
		}
		
		protected virtual bool NodeHasChild(Node node){
			return NodeGetFirstChild(node) != null;
		}
				
		protected virtual Node NodeGetFirstChild(Node node){
			T col = GetCollection(node.NodeVal);
			return Node.CreateFirstChildNode(col,node.Path);
		}		
	}
}

