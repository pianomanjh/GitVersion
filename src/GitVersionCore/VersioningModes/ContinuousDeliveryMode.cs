using System.Collections.Generic;
using System.Linq;
using GitVersion.Extensions;
using LibGit2Sharp;

namespace GitVersion.VersioningModes
{
    public class ContinuousDeliveryMode : VersioningModeBase
    {
        public override SemanticVersionPreReleaseTag GetPreReleaseTag(GitVersionContext context, List<Tag> possibleCommits, int numberOfCommits)
        {
            return RetrieveMostRecentOptionalTagVersion(context, possibleCommits) ?? context.Configuration.Tag + ".1";
        }

        private static SemanticVersionPreReleaseTag RetrieveMostRecentOptionalTagVersion(GitVersionContext context, IReadOnlyCollection<Tag> applicableTagsInDescendingOrder)
        {
            if (applicableTagsInDescendingOrder.Any())
            {
                var taggedCommit = applicableTagsInDescendingOrder.First().PeeledTarget();
                var preReleaseVersion = applicableTagsInDescendingOrder.Select(tag => SemanticVersion.Parse(tag.FriendlyName, context.Configuration.GitTagPrefix)).FirstOrDefault();
                if (preReleaseVersion != null)
                {
                    if (taggedCommit != context.CurrentCommit)
                    {
                        preReleaseVersion.PreReleaseTag.Number++;
                    }
                    return preReleaseVersion.PreReleaseTag;
                }
            }
            return null;
        }
    }
}
