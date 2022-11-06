namespace LoanComparer.Application.Model
{
    public class JobType
    {
        public string Name { get; private set; }
        public virtual IReadOnlyCollection<User> Users { get; private set; }

        public JobType(string name)
        {
            Name = name;
        }

        private JobType() { }
    }
}
