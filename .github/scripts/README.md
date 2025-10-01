# MCP Scripts and Templates Directory Structure

This directory contains extracted scripts and templates for the PFPT MCP (Model Context Protocol) workflows, improving maintainability and reusability.

## Directory Structure

```
.github/
├── scripts/mcp/           # Executable scripts
│   ├── accessibility/     # Accessibility testing scripts
│   ├── database/         # Database validation and migration scripts
│   ├── documentation/    # Documentation generation scripts
│   ├── localization/     # Localization workflow scripts
│   ├── pdf/             # PDF generation and validation scripts
│   ├── release-notes/   # Release notes generation scripts
│   └── triage/          # Issue and PR triage automation
├── templates/            # Template files
│   ├── accessibility/   # Accessibility report templates
│   ├── database/        # Database schema templates
│   ├── documentation/   # Documentation templates
│   ├── localization/    # Localization resource templates
│   ├── pdf/            # PDF report templates
│   └── release-notes/  # Release notes templates
└── workflows/           # GitHub Actions workflows
```

## Benefits of Extraction

### 🔧 **Maintainability**
- **Separated concerns**: Scripts and templates are in dedicated files
- **Version control**: Independent tracking of script changes
- **Code reuse**: Templates can be shared across workflows
- **Easier debugging**: Scripts can be tested independently

### 📝 **Template System**
- **Parameterized content**: Use `{{VARIABLE}}` placeholders
- **Consistent formatting**: Standardized templates across workflows
- **Localization support**: Easy translation of templates
- **Healthcare compliance**: Built-in HIPAA and accessibility considerations

### 🧪 **Testing & Development**
- **Local testing**: Scripts can be run independently
- **Rapid iteration**: Template changes don't require workflow edits
- **IDE support**: Full syntax highlighting and IntelliSense
- **Healthcare validation**: Specialized scripts for clinical workflows

## Usage Examples

### Using Templates
```bash
# Copy template and replace variables
cp .github/templates/accessibility/ui-components-report.md report.md
sed -i "s/{{ACCESSIBILITY_LEVEL}}/AA/g" report.md
sed -i "s/{{TEST_DATE}}/$(date)/g" report.md
```

### Running Scripts Directly
```bash
# Test PDF validation locally
python3 .github/scripts/mcp/pdf/validate-pdf.py

# Run accessibility tests
node .github/scripts/mcp/accessibility/keyboard-nav-test.js

# Validate database setup
python3 .github/scripts/mcp/database/validate-database.py
```

### Healthcare-Specific Features
- **HIPAA compliance**: Built into all templates and scripts
- **Clinical workflow**: Healthcare-specific categorization and validation
- **Accessibility**: WCAG 2.1 compliance throughout
- **PDF compliance**: Healthcare document standards

## Script Categories

### 🏥 **Healthcare & Clinical**
- Patient data validation scripts
- Clinical workflow automation
- HIPAA compliance checking
- Medical terminology validation

### ♿ **Accessibility**
- WCAG 2.1 compliance testing
- Keyboard navigation validation
- Screen reader compatibility
- Healthcare form accessibility

### 📄 **PDF & Documentation**
- Clinical report generation
- PDF accessibility validation
- Documentation automation
- API documentation generation

### 🗄️ **Database & Infrastructure**
- EF Core migration validation
- SQLite schema analysis
- Database performance testing
- Healthcare data integrity checks

All scripts and templates are designed with healthcare compliance and clinical workflows in mind, ensuring that PFPT maintains its focus as a professional physical therapy documentation platform.