namespace ConferencePlanner.Service.GraphQl;

using System.Reflection;

using ConferencePlanner.Data;

using HotChocolate.Types.Descriptors;

internal class UseApplicationDbContextAttribute : ObjectFieldDescriptorAttribute
{
    public override void OnConfigure(IDescriptorContext context, IObjectFieldDescriptor descriptor, MemberInfo member)
    {
        descriptor.UseDbContext<ApplicationDbContext>();
    }
}