# Git Dir Compare Expander

Utility tool to automatically expand `git difftool --dir-diff` into diff instances for the files.

(Useful with Beyond Compare)

## Usage

1. Simple:
    1. Clone repo locally.
    1. Build/Publish tool.
    1. Add `GitParallelDifftool` environment variable that points to a diff tool (GUI preferable) that can run in parallel.
    1. Start tool with `GitDirCompareExpander <LEFT DIR> <RIGHT DIR>`.

1. Advanced:
    1. Follow the first 3 steps in `Simple`.
    1. Add the folder where you built/published the tool to your `PATH` environment variable.
    1. In your `.gitconfig` add this tool as a new diff tool ([e.g.](#diff-tool-section)).
    1. In your `.gitconfig` add `diff-all = "difftool --tool=diff-all-tool --dir-diff"` to your aliases.
    1. Use `git diff-all` to view all changed files.

## Appendix

1. <a name="diff-tool-section"></a> `.gitconfig` diff tool section
```
[difftool "diff-all-tool"]
    cmd = "GitDirCompareExpander $LOCAL $REMOTE"
```