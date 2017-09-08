using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using iayos.extensions;

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

	public static class TestStuff
	{

		///// <summary>
		///// Get a particular Attribute from a particular Method on a particular object instance
		///// </summary>
		///// <typeparam name="TClass"></typeparam>
		///// <typeparam name="TAttribute"></typeparam>
		///// <param name="obj"></param>
		///// <param name="methodName"></param>
		///// <param name="throwOnError"></param>
		///// <returns></returns>
		//public static TAttribute GetAttributeOnMethod<T, TAttribute>(this T obj, string methodName, bool throwOnError = true)
		//	//where TClass : class, new()
		//	where TAttribute : Attribute
		//{
		//	//var type = typeof(obj.GetType());
		//	return AttributeHelper.GetAttributeOnMethod<T, TAttribute>(methodName, throwOnError);
		//}

	}
}
