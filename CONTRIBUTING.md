# Contributing to Stott Security

I am open to contributions to the code base. The following rules should be followed:

1. Add a comment to the issue you are going to be addressing, that way I can mark the issue as in progress to prevent duplication of effort.
2. Contributions should be made by Pull Requests targeting the `develop` branch.
3. All commits should have a meaningful messages.
4. All commits should have a reference to your GitHub user.
5. Ideally all new changes should include appropriate unit test coverage.

## Getting Started

Fork the repository by clicking the **Fork** button at the top of the [repository page](https://github.com/GeekInTheNorth/Stott.Security.Optimizely), then clone your fork locally:

```bash
git clone https://github.com/your-username/Stott.Security.Optimizely.git
cd Stott.Security.Optimizely
```

## Branching

All contributions must branch from `develop`, not `main`. Sync your fork before starting any new work:

```bash
git fetch upstream
git checkout develop
git merge upstream/develop
git checkout -b feature/your-feature-name
```

Pull requests that target `main` may be out of sync with the `develop` branch and may not be compatible for merging.

## Pull Requests

- Keep changes focused — one feature or bug fix per pull request.
- Write clear, descriptive commit messages and reference any related issues (e.g. `Fixes #123`).
- Ensure existing tests pass and include new tests for any changed or added behaviour.
- Verify the application behaves correctly end-to-end before submitting.
- Open your pull request against the `develop` branch of the upstream repository.

## Questions

If you have questions before getting started, either make a comment on the specific [issue](https://github.com/GeekInTheNorth/Stott.Security.Optimizely/issues) or open a [Discussion](https://github.com/GeekInTheNorth/Stott.Security.Optimizely/discussions). All contributions are welcome — thank you for helping improve Stott Security!
