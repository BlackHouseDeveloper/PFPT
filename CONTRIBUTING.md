# Contributing to PFPT

Thank you for contributing to the Physically Fit PT (PFPT) project! This guide will help you maintain code quality and ensure smooth collaboration.

## Before You Start

1. **Read the Documentation**: Familiarize yourself with [ARCHITECTURE.md](ARCHITECTURE.md) and [DEVELOPMENT.md](DEVELOPMENT.md)
2. **Setup Your Environment**: Follow the setup instructions in the README.md

## Development Workflow

### Code Style and Formatting

This project enforces strict code formatting and treats all warnings as errors. To ensure your PR passes CI:

1. **Always run the format script before committing:**
   ```bash
   ./scripts/format.sh
   ```

2. **The format script will:**
   - Format your code using `dotnet format`
   - Build the project with warnings as errors
   - Fail if there are any formatting issues or warnings

3. **Common Rules:**
   - Use 2-space indentation
   - Add final newlines to all files
   - Remove unused `using` statements
   - Follow .NET naming conventions
   - Add XML documentation for public APIs

### Pull Request Process

1. **Create a Feature Branch**: Use descriptive branch names like `feature/add-patient-search` or `fix/memory-leak`

2. **Make Small, Focused Changes**: Keep PRs small and focused on a single concern

3. **Test Locally**: 
   - Run `./scripts/format.sh` to ensure formatting and build pass
   - Run tests with `dotnet test`
   - Test on relevant platforms (Web, Android, iOS)

4. **Fill Out PR Template**: Use the provided PR template and complete all checklist items

5. **CI Must Pass**: All CI checks must be green before merge:
   - **Ensure Format**: Verifies code formatting and no warnings
   - **Build Test**: Builds all platforms and runs tests

### Code Quality Standards

#### Warnings as Errors
All compiler warnings and analyzer warnings are treated as errors:
- Style violations (StyleCop, IDE rules)
- Design issues (.NET analyzers)
- Security and reliability warnings
- Maintainability concerns

#### Analyzer Categories
The following analyzer categories are enforced as errors:
- **Design**: API design best practices
- **Maintainability**: Code complexity and readability
- **Reliability**: Potential runtime issues
- **Security**: Security vulnerabilities
- **Style**: Code style consistency

#### Suppressing Warnings
Only suppress warnings when absolutely necessary:
- Use `#pragma warning disable` for specific lines
- Add suppressions to `.editorconfig` for generated code
- Document why the suppression is needed

### Platform-Specific Considerations

#### WebAssembly (Browser)
- Avoid native dependencies
- Use in-memory storage for browser compatibility
- Test in actual browser environments

#### Mobile (Android/iOS)
- Consider platform-specific features
- Test on actual devices when possible
- Handle platform permissions appropriately

### Common Issues and Solutions

#### Format Script Fails
```bash
# Most common issue: formatting violations
./scripts/format.sh
# Fix: The script will show specific formatting errors to fix

# If build fails with warnings:
dotnet build -warnaserror --verbosity normal
# Fix: Address all warnings shown in the output
```

#### CI Failures
- **Ensure Format fails**: Run `./scripts/format.sh` locally and fix issues
- **Build fails**: Check for platform-specific build issues
- **Tests fail**: Run `dotnet test` locally and fix failing tests

### Getting Help

- **Documentation**: Check existing documentation in the repo
- **Issues**: Search existing issues or create a new one
- **Code Style**: Refer to `.editorconfig` and existing code examples

## Code of Conduct

- Be respectful and constructive in all interactions
- Focus on the code and technical issues, not personal attributes
- Help newcomers learn and contribute effectively
- Follow the project's technical standards consistently

## Thank You!

Your contributions help make PFPT better for physical therapy professionals everywhere. We appreciate your attention to code quality and adherence to these guidelines!