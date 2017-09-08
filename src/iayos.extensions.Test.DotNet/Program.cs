using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iayos.extensions.Test.DotNet
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

			var length = AttributeHelper.GetAttributeOnMethod<Parent, StringLengthAttribute>(nameof(Parent.MyTest));

			//parent.GetAttributeOnProperty<StringLengthAttribute>(x => x.Children);
			var test = AttributeHelper.GetAttributeOnProperty<Parent, StringLengthAttribute>(x => x.Children);

			//var stuff = parent.GetAttributeFrom<StringLengthAttribute>() .GetAttributeOnMethod<StringLengthAttribute>(nameof(Parent.MyTest), true);

			//ICollection<Child> shit;
			//shit = new Collection<Child>();
		}
	}

	public class Parent
	{

		public List<Child> Children { get; set; } = new List<Child>();


		public void MyTest()
		{

		}

	}

	public class Child
	{
		public int ChildId { get; set; }

		public Parent Parent { get; set; }

		[StringLength(12)]
		public string Name { get; set; }

	}
}
