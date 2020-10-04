# Contribution Guidelines

<!-- START doctoc generated TOC please keep comment here to allow auto update -->
<!-- DON'T EDIT THIS SECTION, INSTEAD RE-RUN doctoc TO UPDATE -->
## Table of Contents

- [Prerequisites](#prerequisites)
- [Definition of trivial contributions](#definition-of-trivial-contributions)
- [Code](#code)
  - [Code Style](#code-style)
  - [Dependencies](#dependencies)
  - [Unit tests](#unit-tests)
- [Contributing process](#contributing-process)
  - [Get buyoff or find open community issues or features](#get-buyoff-or-find-open-community-issues-or-features)
  - [Set up your environment](#set-up-your-environment)
  - [Prepare commits](#prepare-commits)
    - [Commit Message Guidelines](#commit-message-guidelines)
      - [Revert](#revert)
      - [Type](#type)
      - [Scope](#scope)
      - [Subject](#subject)
      - [Body](#body)
      - [Footer](#footer)
      - [Breaking Changes](#breaking-changes)
  - [Submit pull request](#submit-pull-request)
  - [Respond to feedback on pull request](#respond-to-feedback-on-pull-request)
  - [Other general information](#other-general-information)
- [Acknowledgement](#acknowledgement)

<!-- END doctoc generated TOC please keep comment here to allow auto update -->

## Prerequisites

By contributing to `CCVARN`, you assert that:

- The contribution is your own original work.
- You have the right to assign the copyright for the work (it is not owned by your employer, or
  you have been given copyright assignment in writing).
- You [license](LICENSE) the contribution under the terms applied to the rest of the project.
- You agree to follow the [code of conduct](CODE_OF_CONDUCT.md).

## Definition of trivial contributions

It's hard to define what is a trivial contribution. Sometimes even a 1 character change can be considered significant.
Unfortunately because it can be subjective, the decision on what is trivial comes from the maintainers of the project
and not from folks contributing to the project.

What is generally considered trivial:

- Fixing a typo.
- Documentation changes.
- Fixes to non-production code - like fixing something small in the build code.

What is generally not considered trivial:

- Changes to any code that would be delivered as part of the final product.
  Yes, even 1 character changes could be considered non-trivial.

## Code

### Code Style

There are analyzers and an `.editorconfig` file available that makes basic decisions of the expected code style being used.
Make sure that any editor you are using are able to make decisions based on these.

### Dependencies

All projects deployed to users whouls have just the minimum of dependencies required for all code to function properly.

### Unit tests

Make sure to run all unit tests before creating a pull request.
Any new code should also have reasonable unit test coverage.

## Contributing process

### Get buyoff or find open community issues or features

- Through GitHub you create an issue where you talk about the feature you would like to see (or a bug), and why it should be in
  project
- Once you get a nod from one of the [WormieCorp Team Members](https://github.com/WormieCorp?tab=members), you can start on the feature.
- Alternatively, if a feature is on the issues list with the
  [Up For Grabs](https://github.com/WormieCorp/CCVARN/labels/up%20for%20grabs) label,
  it is open for community member (contributor) to patch. You should comment that you are signing up for it on
  the issue so someone else doesn't also sign up for the work.
- If there is already a different user assigned to the issue, do **NOT** pick up the issue without getting permission.

### Set up your environment

- You create, or update, a fork of the project under your GitHub account.
- From there you create a branch named specific to the feature.
- In the branch you do work specific to the feature.
- Please also observe the following:
  - No reformatting
  - No changing files that are not specific to the feature.
  - More covered below in the **Prepare commits** section.
- Test your changes and please help us out by updating and implementing some automated tests.
  It is recommended that all contributors spend some time looking over the tests in the source code.
  You can't go wrong emulating one of the existing tests and then changing it specific to the behaviour you are testing.
- Please do not update your branch from the develop unless we ask you to. See the responding to feedback section below.

### Prepare commits

This section serves to help you understand what makes a good commit.

A commit should observe the following:

- A commit is a small logical unit that represents a change.
- Should include new or changed tests relevant to the changes you are making.
- No unnecessary whitespace. Check for whitespace with `git diff --check` and `git diff --cached --check` before commit.
- You can stage parts of a file for commit.
- All commits should be based on the [conventional commits](https://www.conventionalcommits.org/en/v1.0.0/) spec.

#### Commit Message Guidelines

The CCVARN project loosely follows the [Angular Convention](https://github.com/angular/angular/blob/22b96b9/CONTRIBUTING.md#-commit-message-guidelines) guidelines.

All commits submitted to the repository is expected to follow the convention of using a minimum of a `type` and a `subject` in the commit summary message.
A commit can also additionally use an optional `scope`, `body` and `footer` section as well (_body is required for breaking changes_).

A typical commit with both a body and a footer will look something similar to:
_NOTE: if a scope in not used in the commit, please also remove the paranthesis_

```console
<type>(<scope>): <subject>
BLANK_LINE
<body>
<BLANK_LINE>
<footer>
```

##### Revert

If the commit reverts a previous commit, it should begin with `revert:`, followed by the header of the reverted commit.
In the body it should say: `This reverts commit <hash>.`, where the hash is the SHA of the commit being reverted.

##### Type

Where the type would be one of the following:

- `feat` (Used for most commits that implements new or improves existing features).
- `improvement` (Used when existing feature is improved, without modifying existing functionality and is not a performance improvement)
- `fix` (Used when a commit fixes an existing bug in the code).
- `perf` (Used when a commit improves the performance of the code, but without changing or adding any new functionality).
- `docs` (Commits only changing any documentation related files).
- `build` (Commits only related to the building of the project)
- `ci` (Commits only changing building on a CI)
- `refactor` (Commits that refactors internal code, this can be extracting code to a seperate method or rename an internal method).
  _NOTE: Any renaming of a public facing method or class is considerd a breaking change_
- `revert` (Any changes that reverts an existing commit)
- `test` (Changes made to only the unit/feature tests)
- `chore` (Anything not matching any of the above types).

##### Scope

There are no hard requirement on using scopes in the commit message.
However, some useful scopes would be:

- `gha` - Only valid when using the `ci` type, and is used when changes is made for github actions
- `export` - Changes that is only related directly to exporting version or release notes.
- `version` - Changes related to the parsing of version.
- `release-note` - Changes related to how the release notes are parsed.

You are free to use additional scopes, or omit them if you so desire.

##### Subject

The subject contains a succinct description of the change

- use the imperative, present tense: "change" not "changed" nor "changes"
- do not capitalize the first letter
- no dot (.) at the end

##### Body

Just as in the **subject**, use the imperative, present tense: "change" not "changed" nor "changes". The body should include the motivation for the change and contrast this with previous behavior (_Optional when a commit is not a breaking change_).

##### Footer

The footer should only be used when the commit is related to an already existing issue.
Only two forms are accepted when referencing issues in the footer.

- `fix #<num>` - When the commit is fixing a bug as detailed by the issue
- `resolve #<num>` - When the commit is adding/updating a feature as detailed by the issue

##### Breaking Changes

When adding breaking changes, the format of the commit is very similiar to any normal commit.
There are a few additional requirements when adding breaking changes however:

- After the type (_ond scope) but before the colon add a `!` to denote the commit is a breaking change.
- A body is required for such commits, the commit will be part of the release notes.
- A issue is required to be referenced in the footer for any breaking changes. This issue must also have been approved by the maintainers before any work is done for a breaking change.

### Submit pull request

Prerequisites:

- You are making commits in a feature branch.
- All code should compile without errors or warnings (excluding any warnings already present in the repository).
- All tests should be passing.

Submitting PR:

- Once you feel it is ready, submit the pull request to the main project repository against the `master` branch
  unless specifically requested to submit it against another branch.
  - In the case of a larger change that is going to require more discussion,
    please submit a PR sooner. Waiting until you are ready may mean more changes that you are
    interested in if the changes are taking things in a direction the maintainers do not want to go.
- In the pull request, outline what you did and point to specific conversations (as in URLs)
  and issues that you are resolving. This is a tremendous help for us in evaluation and acceptance.
- Once the pull request is in, please do not delete the branch or close the pull request
  (unless something is wrong with it).
- One of the WormieCorp team members, or one of the maintainers, will evaluate it within a
  reasonable timespan (which is to say usually within 1-3 weeks). Some things get evaluated
  faster or fast tracked. We are human and we have active lives outside of open source so don't
  fret if you haven't seen any activity on your pull request within a month or two.
  We don't have a Service Level Agreement (SLA) for pull requests.
  Just know that we will evaluate your pull request.

### Respond to feedback on pull request

We may have feedback for you to fix or change some things. We generally like to see that pushed against
the same topic branch (it will automatically update the Pull Request). You can also fix/squash/rebase
commits and push the same topic branch with `--force` (it's generally acceptable to do this on topic
branches not in the main repository), it is generally unacceptable and should be avoided at all costs
against the main repository).

If we have comments or questions when we do evaluate it and receive no response, it will probably
lessen the chance of getting accepted. Eventually, this means it will be closed if it is not accepted.
Please know this doesn't mean we don't value your contribution, just that things go stale. If in the
future you want to pick it back up, feel free to address our concerns/questions/feedback and reopen
the issue/open a new PR (referencing the old one).

Sometimes we may need you to rebase your commits against the latest code before we can review it further.
If this happens, you can do the following:

- `git fetch upstream` (upstream would be the mainstream repo)
- `git checkout master`
- `git rebase upstream/master`
- `git checkout your-branch`
- `git rebase master`
- Fix any merge conflicts
- `git push origin your-branch` (origin would be your GitHub repo).
  You may need to `git push origin your-branch --force` to get the commits pushed.
  This is generally acceptable with topic branches not in the mainstream repository.

The only reasons a pull request should be closed and resubmitted are as follows:

- When the pull request is targeting the wrong branch (this doesn't happen as often).
- When there are updates made to the original by someone other than the original contributor.
  The the old branch is closed with a note on the newer branch this supersedes #github_number.

### Other general information

If you reformat code or hit core functionality without an approval from a person on the WormieCorp Team,
it's likely that no matter how awesome it looks afterwards, it will probably not get accepted.
Reformatting code makes it more challenging for us to evaluate exactly what was changed.

If you do these things, it will make evaluation and acceptance easy.
Now if you stray outside of the guidelines we have above, it doesn't mean we are going to ignore
your pull request. It will just make things more challenging for us.
More challenging for us roughly translates to a longer SLA for your pull request.

## Acknowledgement

This contribution guide was taken from [Cake project](http://cakebuild.net/)
with permission and was edited to follow CCVARN's conventions and processes.
