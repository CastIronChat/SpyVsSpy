/// <reference path="./deno.d.ts" />
import * as fs from "https://deno.land/std@0.60.0/fs/mod.ts";
import * as Path from "https://deno.land/std/path/mod.ts";

const url = new URL(import.meta.url);
const projectRoot = Path.dirname(Path.dirname(url.pathname));
const assetsPath = Path.join(projectRoot, 'Assets').replace(/^\\([A-Z]:\\)/, '$1');
console.dir(assetsPath);
for (const result of fs.walkSync(assetsPath, {
    includeDirs: true,
    includeFiles: false,
    followSymlinks: false,
})) {
    const { path: directoryPath } = result;
    if (Path.basename(directoryPath) === 'Editor') {
        const asmdefPath = Path.join(directoryPath, '_.asmdef');
        const assemblyPath = Path.relative(assetsPath, directoryPath);
        const assemblyName = assemblyPath.replace(/[\\\/]/g, '.').replace(/ /g, '_');
        console.log(assemblyName);
        if (!fs.existsSync(asmdefPath)) {
            fs.writeJsonSync(asmdefPath, {
                "name": assemblyName,
                "references": [],
                "includePlatforms": [],
                "excludePlatforms": [],
                "allowUnsafeCode": false,
                "overrideReferences": false,
                "precompiledReferences": [],
                "autoReferenced": false,
                "defineConstraints": [
                    "UNITY_EDITOR"
                ],
                "versionDefines": []
            });
        }
    }
};
// fs.walkSync()
