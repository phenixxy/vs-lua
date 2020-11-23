using System.ComponentModel.Composition;
using Microsoft.VisualStudio.LanguageServer.Client;
using Microsoft.VisualStudio.Utilities;

namespace VSLua
{
    public class ContentDefinition
    {
        [Export]
        [Name("lua")]
        [BaseDefinition(CodeRemoteContentDefinition.CodeRemoteContentTypeName)]
        internal static ContentTypeDefinition ContentTypeDefinition;


        [Export]
        [ContentType("lua")]
        [FileExtension(".lua")]
        internal static FileExtensionToContentTypeDefinition FileExtensionDefinition;
    }
}
