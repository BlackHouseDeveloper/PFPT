# PFPT Developer Onboarding Guide

Welcome to the Physically Fit PT (PFPT) development team! This guide will help you get up and running quickly.

## Prerequisites

### Required Software
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Visual Studio Code](https://code.visualstudio.com/) or Visual Studio
- [Git](https://git-scm.com/)

### Platform-Specific Requirements
- **Android Development**: Android SDK, OpenJDK 17
- **iOS Development**: Xcode (macOS only)
- **Windows Development**: Windows SDK

### Recommended Extensions (VS Code)
- C# for Visual Studio Code
- .NET MAUI
- GitLens
- REST Client

## Quick Start (5 minutes)

1. **Clone the repository**:
   ```bash
   git clone https://github.com/BlackHouseDeveloper/PFPT.git
   cd PFPT
   ```

2. **Run the setup script**:
   ```bash
   ./PFPT-Foundry.sh
   ```

3. **Open in VS Code**:
   ```bash
   code .
   ```

4. **Validate your setup**:
   ```bash
   gh workflow run mcp-copilot-setup-validation.yml
   ```

## Healthcare Development Guidelines

### HIPAA Compliance
- Never commit patient data to version control
- Use encryption for all sensitive data
- Implement proper access controls
- Maintain audit logs for PHI access

### Clinical Workflow Considerations
- Test with real clinical scenarios
- Validate medical terminology accuracy
- Ensure accessibility for healthcare professionals
- Consider workflow efficiency in design decisions

### Quality Assurance
- Run accessibility tests: `gh workflow run mcp-accessibility-compliance.yml`
- Validate PDF generation: `gh workflow run mcp-pdf-diagnostics.yml`
- Test database migrations: `gh workflow run mcp-database-diagnostics.yml`

## MCP Workflows

PFPT includes 11+ automated workflows for development assistance:

- **Database**: Migration and validation automation
- **PDF**: Report generation and accessibility testing
- **Accessibility**: WCAG 2.1 compliance validation
- **Documentation**: Auto-generation and maintenance
- **Localization**: Multi-language support automation
- **Error Reproduction**: Debugging assistance
- **Setup Validation**: Environment verification
- **Release Notes**: Automated changelog generation
- **Auto-formatting**: Code style maintenance
- **Triage**: Issue and PR automation

## Learning Resources

1. **Architecture**: [docs/ARCHITECTURE.md](../docs/ARCHITECTURE.md)
2. **Development**: [docs/DEVELOPMENT.md](../docs/DEVELOPMENT.md)
3. **CI/CD**: [docs/CI.md](../docs/CI.md)
4. **Copilot Guide**: [.github/Copilot-Instructions.md](../.github/Copilot-Instructions.md)
5. **Agent Guide**: [PFPT/Agents.md](PFPT/Agents.md)

## Getting Help

- 💬 **Discussions**: Use GitHub Discussions for questions
- 🐛 **Issues**: Report bugs using our issue templates
- 📝 **Documentation**: Check the docs/ directory
- 🤖 **MCP Workflows**: Use automated diagnostics for common problems

Welcome to the team! 🏥