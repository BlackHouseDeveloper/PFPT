# MCP Templates Index

This directory contains template files for PFPT MCP workflows. Templates use `{{VARIABLE}}` placeholders that are replaced during workflow execution.

## Template Categories

### 📄 **Documentation Templates**
- `api-docs-template.md` - API documentation structure
- `onboarding-template.md` - Developer onboarding guide
- `project-structure-template.md` - Architecture documentation

### ♿ **Accessibility Templates**
- `axe-config.json` - Accessibility testing configuration
- `ui-components-report.md` - UI accessibility report format

### 📝 **Release Notes Templates**
- `generation-context-template.md` - Release context information

## Template Variables

### Common Variables
- `{{GENERATION_DATE}}` - Current UTC timestamp
- `{{PROJECT_NAME}}` - PFPT project name
- `{{VERSION}}` - Current version

### Release Notes Variables
- `{{RELEASE_TYPE}}` - Type of release (feature, hotfix, etc.)
- `{{FROM_TAG}}` - Previous version tag
- `{{TO_TAG}}` - Current version tag
- `{{INCLUDE_CLINICAL}}` - Clinical categorization flag

### Accessibility Variables
- `{{ACCESSIBILITY_LEVEL}}` - WCAG level (A, AA, AAA)
- `{{TEST_DATE}}` - Test execution date
- `{{VIOLATIONS_COUNT}}` - Number of violations found
- `{{PASSES_COUNT}}` - Number of rules passed
- `{{CRITICAL_ISSUES}}` - Critical accessibility issues

### Documentation Variables
- `{{PROJECT_TREE}}` - Project structure tree
- `{{GENERATION_DATE}}` - Documentation generation timestamp

## Healthcare Compliance

All templates include:
- HIPAA compliance considerations
- Clinical workflow context
- Accessibility requirements (WCAG 2.1)
- Professional healthcare documentation standards

## Usage

Templates are automatically processed by MCP workflows. Variables are replaced using `sed` or similar tools during workflow execution.

Example:
```bash
cp .github/templates/accessibility/ui-components-report.md report.md
sed -i "s/{{ACCESSIBILITY_LEVEL}}/AA/g" report.md
sed -i "s/{{TEST_DATE}}/$(date)/g" report.md
```