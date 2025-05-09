# Template to PDF UI

A React-based frontend for the Template to PDF application. Built with React Admin, Material-UI, and TypeScript.

## Prerequisites

- Node.js 18 or later
- npm 9 or later
- Backend API running (see main project README)

## Getting Started

1. Install dependencies:
```bash
npm install
```

2. Set up environment variables:
   - Copy `.env.development` to `.env.local` for local development
   - Adjust API URL if needed in `.env.local`:
```env
REACT_APP_API_URL=https://localhost:6051/api
PORT=3000
```

3. Start the development server:
```bash
npm start
```

The application will be available at [http://localhost:3000](http://localhost:3000).

## Using Assets in Templates

You can reference assets in your templates using their reference names. The reference name is automatically generated from the asset name (lowercase, hyphens instead of spaces) or can be manually specified.

### Available Asset Types

1. CSS Stylesheets:
```handlebars
{{css "main-styles"}}
```

2. Images:
```handlebars
{{image "company-logo"}}
```

3. Fonts:
```handlebars
{{font "custom-font"}}
<div style="font-family: 'custom-font'">Text using custom font</div>
```

4. Partial Templates:
```handlebars
{{> "invoice-header"}}
```

### Reference Name Examples
- Asset name: "Main Stylesheet" → Reference name: "main-stylesheet"
- Asset name: "Company Logo.png" → Reference name: "company-logo"
- Asset name: "Open Sans Bold" → Reference name: "open-sans-bold"
- Asset name: "Invoice Header" → Reference name: "invoice-header"

### Best Practices
- Use descriptive asset names
- Reference names are automatically converted to URL-friendly format
- Reference names can only contain lowercase letters, numbers, hyphens and underscores
- Keep reference names short but meaningful

## Project Structure

```
src/
├── components/         # React components
│   ├── templates/     # Template management components
│   ├── settings/      # Settings page components
│   └── Dashboard.tsx  # Dashboard component
├── i18n/              # Internationalization files
│   ├── fi.ts         # Finnish base translations
│   └── sv.ts         # Swedish base translations
├── layout/           # Layout components
├── App.tsx          # Main application component
├── dataProvider.ts  # React Admin data provider
└── index.tsx        # Application entry point
```

## Features

- Template Management
  - Create, edit, and delete HTML templates
  - Preview templates
  - Generate PDFs from templates
- Asset Management
  - Upload and manage CSS, images, fonts, and partial templates
  - Reference assets in templates using friendly names
- Multi-language Support
  - English (default)
  - Finnish
  - Swedish
- Theme Support
  - Light theme
  - Dark theme
- Responsive Design
- Material-UI Components

## Available Scripts

- `npm start` - Start development server
- `npm build` - Build production version
- `npm test` - Run tests
- `npm run eject` - Eject from Create React App

## Development

### Code Style

The project uses Prettier for code formatting. Configuration is in `.prettierrc`.

### Adding New Features

1. Create new components in the appropriate directory under `src/components/`
2. Add translations for new features in `src/i18n/i18nProvider.ts`
3. Update the main `App.tsx` if needed
4. Add any new routes to the React Admin configuration

### Internationalization

All translations should be added to `src/i18n/i18nProvider.ts`. The file contains translations for all supported languages (en, fi, sv) in a structured format.

To add new translations:

1. Add translation keys and values in the `messages` object in `i18nProvider.ts`:
```typescript
const messages = {
    en: {
        ...en,
        your: {
            new: {
                key: 'English translation'
            }
        }
    },
    fi: {
        ...fi,
        your: {
            new: {
                key: 'Finnish translation'
            }
        }
    },
    sv: {
        ...sv,
        your: {
            new: {
                key: 'Swedish translation'
            }
        }
    }
};
```

2. Use the `useTranslate` hook in components:
```typescript
import { useTranslate } from 'react-admin';

const MyComponent = () => {
    const translate = useTranslate();
    return <div>{translate('my.translation.key')}</div>;
};
```

### API Integration

The application uses React Admin's data provider to communicate with the backend API. See `src/dataProvider.ts` for implementation details.

## Building for Production

1. Update environment variables in `.env.production`
2. Build the project:
```bash
npm run build
```
3. The production build will be in the `build/` directory
4. Deploy the contents of `build/` to your web server

## Contributing

1. Create a feature branch
2. Make your changes
3. Run tests
4. Submit a pull request

## License

MIT 