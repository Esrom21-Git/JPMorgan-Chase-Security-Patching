# Contributing to Automated .NET Security Patching System

Thank you for your interest in contributing! This document provides guidelines for contributing to this project.

## 🤝 How to Contribute

### Reporting Bugs

If you find a bug, please create an issue with:
- Clear description of the bug
- Steps to reproduce
- Expected behavior
- Actual behavior
- Your environment (.NET version, OS, etc.)

### Suggesting Enhancements

Enhancement suggestions are welcome! Please:
- Check existing issues first
- Provide clear use case
- Explain expected benefits
- Include examples if possible

### Pull Requests

1. **Fork the repo** and create your branch from `main`
2. **Make your changes** with clear, descriptive commits
3. **Test your changes** thoroughly
4. **Update documentation** if needed
5. **Submit a pull request**

## 💻 Development Setup

```bash
# Clone your fork
git clone https://github.com/YOUR-USERNAME/security-patching-automation.git
cd security-patching-automation

# Set up environment variables
export GITHUB_TOKEN="your-test-token"
export GITHUB_ORG="your-test-org"

# Build and test
dotnet build
dotnet run -- --dry-run
```

## 📝 Coding Standards

- Follow C# naming conventions
- Use meaningful variable names
- Add comments for complex logic
- Keep methods focused and concise
- Handle errors gracefully

## 🧪 Testing

Before submitting:
- Run in `--dry-run` mode
- Test with various repository configurations
- Verify error handling
- Check console output formatting

## 📋 Commit Message Guidelines

- Use present tense ("Add feature" not "Added feature")
- Use imperative mood ("Move cursor to..." not "Moves cursor to...")
- Reference issues and pull requests
- First line: brief summary (50 chars max)
- Detailed description if needed

Examples:
```
Add support for npm packages

Fix PR creation for private repos (fixes #123)

Improve error logging with stack traces
```

## 🔍 Code Review Process

All PRs will be reviewed for:
- Code quality and style
- Test coverage
- Documentation updates
- Breaking changes
- Security considerations

## 📜 License

By contributing, you agree that your contributions will be licensed under the MIT License.

## ❓ Questions?

Feel free to open an issue for any questions!
