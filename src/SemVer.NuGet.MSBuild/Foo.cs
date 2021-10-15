// Copyright © William Sugarman.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis.MSBuild;

namespace SemVer.NuGet.MSBuild
{
    internal class Foo
    {
        public void Bar()
        {
            using (var workspace = MSBuildWorkspace.Create())
            {
                var project = await workspace.OpenProjectAsync("MyProject.csproj");
                var compilation = await project.GetCompilationAsync();

                // Perform analysis...
            }
        }
    }
}
