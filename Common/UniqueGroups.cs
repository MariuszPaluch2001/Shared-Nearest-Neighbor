namespace SNN.Common
{
    public class UniqueGroups
    {
        public static int UniqueGroupsCount<T>(IList<T> group)
        {
            return group.Distinct().Count();
        }
    }
}
