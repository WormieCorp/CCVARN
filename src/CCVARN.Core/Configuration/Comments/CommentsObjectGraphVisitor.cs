namespace CCVARN.Core.Configuration.Comments
{
	using YamlDotNet.Core;
	using YamlDotNet.Core.Events;
	using YamlDotNet.Serialization;
	using YamlDotNet.Serialization.ObjectGraphVisitors;

	internal sealed class CommentsObjectGraphVisitor : ChainedObjectGraphVisitor
	{
		public CommentsObjectGraphVisitor(IObjectGraphVisitor<IEmitter> nextVisitor)
			: base(nextVisitor)
		{
		}

		public override bool EnterMapping(IPropertyDescriptor key, IObjectDescriptor value, IEmitter context)
		{
			if (value is CommentsObjectDescriptor commentsDescriptor && commentsDescriptor.Comment != null)
			{
				foreach (var comment in commentsDescriptor.Comment.Split('\n'))
				{
					context.Emit(new Comment(comment, false));
				}
			}

			return base.EnterMapping(key, value, context);
		}
	}
}
