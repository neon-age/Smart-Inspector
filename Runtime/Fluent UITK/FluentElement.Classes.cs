
namespace AV.UITK
{
    public partial class FluentElement<T>
    {
        public FluentElement<T> Name(string name)
        {
            x.name = NamePokaYoke(name);
            return x;
        }
        
        public bool HasClass(string className)
        {
            return x.ClassListContains(ClassPokaYoke(className));
        }
        public FluentElement<T> AddClass(params string[] classes)
        {
            foreach (var className in classes)
                x.AddToClassList(ClassPokaYoke(className));
            
            return x;
        }
        public FluentElement<T> RemoveClass(params string[] classes)
        {
            foreach (var className in classes)
                x.RemoveFromClassList(ClassPokaYoke(className));
            
            return x;
        }
        public FluentElement<T> EnableClass(string className, bool enable)
        {
            x.EnableInClassList(ClassPokaYoke(className), enable);
            return x;
        }
        
        
        string NamePokaYoke(string name)
        {
            if (name.StartsWith("#"))
                return name.Remove(0, 1);
            return name;
        }
        string ClassPokaYoke(string className)
        {
            if (className.StartsWith("."))
                return className.Remove(0, 1);
            return className;
        }
    }
}