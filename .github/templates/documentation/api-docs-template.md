# PFPT API Documentation

_Auto-generated on {{GENERATION_DATE}}_

## Overview

This documentation covers the core APIs and services for the Physically Fit PT (PFPT) application.

## Projects

### Core Domain
- [Entities](./entities/)
- [Value Objects](./value-objects/)
- [Domain Services](./domain-services/)

### Infrastructure Layer
- [Data Access](./data-access/)
- [External Services](./external-services/)
- [PDF Generation](./pdf-generation/)

### Application Layer
- [Service Interfaces](./interfaces/)
- [Core Services](./services/)

## Usage Guidelines

- All public APIs include XML documentation
- Follow dependency injection patterns
- Use async/await for I/O operations
- Implement proper error handling
- Maintain healthcare compliance (HIPAA)
- Ensure accessibility in UI components

## Healthcare Compliance

All APIs that handle patient data must:
- Implement proper authentication and authorization
- Log access for audit trails
- Encrypt sensitive data at rest and in transit
- Follow HIPAA guidelines for PHI handling

## Getting Started

1. Review the [Architecture Documentation](../docs/ARCHITECTURE.md)
2. Set up the development environment using [PFPT-Foundry.sh](../PFPT-Foundry.sh)
3. Run the [MCP Setup Validation](../.github/workflows/mcp-copilot-setup-validation.yml)
4. Follow the [Development Guide](../docs/DEVELOPMENT.md)

_Last updated: {{GENERATION_DATE}}_