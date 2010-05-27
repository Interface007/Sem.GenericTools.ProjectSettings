namespace Sem.GenericTools.CodeAnalysis
{
    using Microsoft.FxCop.Sdk;

    public abstract class SemRule : BaseIntrospectionRule
    {
        protected SemRule(string name)
            : base(
                name,                                                                   // The name of the rule (must match the entry in the Rules.XML)
                string.Format("{0}.Rules", typeof(SemRule).Assembly.GetName().Name),    // The name of the Rules.XML file in this project
                typeof(SemRule).Assembly)                                               // The assembly to find the Rules XML in
        {
        }

        /// <summary>
        /// Adds a problem to the ProblemList. 
        /// Certainty, FixCategory and messageLevel are set automatically.
        /// </summary>
        /// <param name="name">the Name of the Faulting thing</param>
        /// <param name="target">the Node of the Thing that has caused the error</param>
        protected void AddProblem(string name, Node target)
        {
            // Get the Resolution - the Messagetext that is shown in the Error-List in VS. 
            // Works like string.Format and inserts the given name into the Errortext defined in the Rules.xml
            var resolution = GetResolution(name);
            
            // Create a new Problem with the resolution and the target node given
            var problem = new Problem(resolution, target)
            {
                Certainty = 100,
                FixCategory = FixCategories.NonBreaking,
                MessageLevel = MessageLevel.Warning
            };

            // Add the Problem to the Problemlist
            Problems.Add(problem);
        }
    }
}
