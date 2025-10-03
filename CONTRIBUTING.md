# Contributing to Job Application Tracker

Thank you for your interest in contributing to the **Job Application Tracker** project for **Hacktoberfest 2025**! 🎉

This document provides guidelines and information for contributors who want to help improve this project.

## 🎯 Hacktoberfest 2025

This project is participating in **Hacktoberfest 2025**! Your contributions can help you earn the official Hacktoberfest 2025 digital badge and swag.

### Hacktoberfest Guidelines
- **Quality over Quantity**: We value meaningful contributions over spam
- **Follow the Rules**: Ensure your PRs follow our contribution guidelines
- **Be Patient**: Maintainers will review PRs as quickly as possible
- **Respect the Process**: Follow the PR template and labeling system

## 🚀 Getting Started

### Prerequisites
- [Git](https://git-scm.com/)
- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0) (for backend)
- [Node.js 18+](https://nodejs.org/) (for frontend)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)
- [Visual Studio Code](https://code.visualstudio.com/) or [Visual Studio](https://visualstudio.microsoft.com/)

### Development Setup

1. **Fork the repository**
   ```bash
   # Click the "Fork" button on GitHub, then clone your fork
   git clone https://github.com/YOUR_USERNAME/Job-Application-Tracker.git
   cd Job-Application-Tracker
   ```

2. **Add upstream remote**
   ```bash
   git remote add upstream https://github.com/ravindu0823/Job-Application-Tracker.git
   ```

3. **Create a feature branch**
   ```bash
   git checkout -b feature/your-feature-name
   # or
   git checkout -b fix/your-bug-fix
   # or
   git checkout -b docs/your-documentation-update
   ```

4. **Set up the development environment**
   ```bash
   # Backend setup
   cd LiterateWinnerApi/LiterateWinnerApi
   dotnet restore
   dotnet build
   
   # Frontend setup (when available)
   cd ../../job-application-tracker-frontend
   npm install
   npm run dev
   ```

## 📋 How to Contribute

### Types of Contributions

We welcome various types of contributions:

#### 🐛 Bug Reports
- Use the **Bug Report** issue template
- Provide detailed reproduction steps
- Include screenshots if applicable
- Test on the latest version

#### ✨ Feature Requests
- Use the **Feature Request** issue template
- Describe the problem and proposed solution
- Consider the impact on existing functionality
- Check if similar features already exist

#### 📚 Documentation
- Improve existing documentation
- Add code comments and examples
- Create tutorials or guides
- Fix typos and grammar

#### 🧪 Testing
- Add unit tests for new features
- Improve test coverage
- Add integration tests
- Performance testing

#### 🎨 UI/UX Improvements
- Design improvements
- Accessibility enhancements
- Mobile responsiveness
- User experience optimizations

### Contribution Process

1. **Find an Issue**
   - Look for issues labeled `good first issue` or `help wanted`
   - Check the Hacktoberfest 2025 issues
   - Comment on the issue to express interest

2. **Create Your Branch**
   ```bash
   git checkout -b feature/issue-number-description
   ```

3. **Make Your Changes**
   - Follow the coding standards
   - Write clear commit messages
   - Add tests if applicable
   - Update documentation

4. **Test Your Changes**
   ```bash
   # Backend tests
   dotnet test
   
   # Frontend tests (when available)
   npm test
   ```

5. **Commit Your Changes**
   ```bash
   git add .
   git commit -m "feat: add new feature for issue #123"
   ```

6. **Push and Create PR**
   ```bash
   git push origin feature/issue-number-description
   # Then create a Pull Request on GitHub
   ```

## 📝 Coding Standards

### General Guidelines
- **Write clean, readable code**
- **Follow existing code patterns**
- **Add comments for complex logic**
- **Use meaningful variable and function names**
- **Keep functions small and focused**

### Backend (.NET 9.0)
- Follow [C# Coding Conventions](https://docs.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions)
- Use dependency injection
- Implement proper error handling
- Add XML documentation for public APIs
- Follow async/await patterns

### Frontend (Next.js 15)
- Follow [React Best Practices](https://react.dev/learn)
- Use TypeScript for type safety
- Follow [Next.js conventions](https://nextjs.org/docs)
- Use Tailwind CSS for styling
- Implement responsive design

### Git Commit Messages
Use the [Conventional Commits](https://www.conventionalcommits.org/) format:

```
type(scope): description

feat(auth): add JWT token refresh functionality
fix(api): resolve CORS issue with frontend
docs(readme): update installation instructions
test(auth): add unit tests for login service
```

**Types**: `feat`, `fix`, `docs`, `style`, `refactor`, `test`, `chore`

## 🏷️ Issue and PR Labels

### Issue Labels
- `hacktoberfest` - Eligible for Hacktoberfest 2025
- `good first issue` - Perfect for newcomers
- `help wanted` - Community help needed
- `bug` - Something isn't working
- `enhancement` - New feature or request
- `documentation` - Documentation improvements
- `backend` - Backend (.NET) related
- `frontend` - Frontend (Next.js) related

### PR Labels
- `hacktoberfest-accepted` - Accepted for Hacktoberfest 2025
- `ready-for-review` - Ready for maintainer review
- `needs-changes` - Requires changes before merge
- `breaking-change` - Contains breaking changes

## 🧪 Testing Guidelines

### Backend Testing
```bash
# Run all tests
dotnet test

# Run specific test project
dotnet test LiterateWinnerApi.Tests

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"
```

### Frontend Testing
```bash
# Run unit tests
npm test

# Run e2e tests
npm run test:e2e

# Run with coverage
npm run test:coverage
```

## 📖 Documentation

### Code Documentation
- Add XML comments for public APIs
- Include examples in documentation
- Update README.md for major changes
- Document configuration options

### API Documentation
- Update Swagger/OpenAPI documentation
- Include request/response examples
- Document error codes and messages
- Add authentication requirements

## 🔍 Review Process

### Pull Request Review
1. **Automated Checks**: CI/CD pipeline runs tests and checks
2. **Code Review**: Maintainers review code quality and functionality
3. **Testing**: Verify changes work as expected
4. **Documentation**: Ensure documentation is updated
5. **Approval**: Maintainer approves and merges

### Review Criteria
- **Functionality**: Does it work as intended?
- **Code Quality**: Is the code clean and maintainable?
- **Testing**: Are there adequate tests?
- **Documentation**: Is documentation updated?
- **Performance**: Does it impact performance?
- **Security**: Are there security implications?

## 🚫 What Not to Contribute

### Spam Contributions
- Automated or scripted contributions
- Duplicate content
- Irrelevant changes
- Whitespace-only changes
- Translation changes without context

### Inappropriate Content
- Offensive or inappropriate language
- Copyrighted material without permission
- Personal information or credentials
- Malicious code or security vulnerabilities

## 🆘 Getting Help

### Resources
- **Documentation**: Check the [Wiki](https://github.com/ravindu0823/Job-Application-Tracker/wiki)
- **Issues**: Search existing issues or create new ones
- **Discussions**: Use GitHub Discussions for questions

### Contact
- **Email**: guestpc87@gmail.com
- **GitHub**: [@ravindu0823](https://github.com/ravindu0823)
- **Website**: [ravinduperera.vercel.app](https://ravinduperera.vercel.app/)

## 🎉 Recognition

### Contributors
- All contributors will be listed in the README
- Significant contributors may be added as maintainers
- Hacktoberfest 2025 participants will be recognized

### Hacktoberfest 2025
- Valid PRs will be marked with `hacktoberfest-accepted`
- Contributors will earn the official Hacktoberfest 2025 badge
- Special recognition for quality contributions

## 📄 License

By contributing to this project, you agree that your contributions will be licensed under the [MIT License](LICENSE).

---

## 🎯 Quick Checklist for Contributors

- [ ] I have read and understood the Code of Conduct
- [ ] I have checked for existing issues and PRs
- [ ] I have created a feature branch for my changes
- [ ] I have followed the coding standards
- [ ] I have added tests for my changes
- [ ] I have updated documentation as needed
- [ ] I have tested my changes locally
- [ ] I have created a clear and descriptive PR
- [ ] I have linked my PR to the related issue

**Thank you for contributing to the Job Application Tracker project! Together, we're building something amazing for Hacktoberfest 2025! 🚀**

---

*This contributing guide is updated for Hacktoberfest 2025. Last updated: October 2024*
