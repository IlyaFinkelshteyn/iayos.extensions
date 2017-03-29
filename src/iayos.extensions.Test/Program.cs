using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iayos.extensions.Test
{
	class Program
	{
		static void Main(string[] args)
		{
			var parent = new Parent();
			var child = new Child { ChildId = 1, Name = "CharlieHorse" };

			parent.Children.InsertOrSet(child, c => c.ChildId == child.ChildId);

			var secondchild = new Child { ChildId = 1, Name = "Rodrigo" };
			parent.Children.InsertOrSet(secondchild, c => c.ChildId == secondchild.ChildId);


			

			//ICollection<Child> shit;
			//shit = new Collection<Child>();
		}
	}

	public class Parent
	{

		public List<Child> Children { get; set; } //= new List<Child>();

	}

	public class Child
	{
		public int ChildId { get; set; }

		public Parent Parent { get; set; }

		public string Name { get; set; }

	}
}
