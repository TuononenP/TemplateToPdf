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

## Project Structure

```
src/
├── components/         # React components
│   ├── templates/     # Template management components
│   ├── settings/      # Settings page components
│   └── Dashboard.tsx  # Dashboard component
├── i18n/              # Internationalization files
│   ├── fi.ts         # Finnish translations
│   └── sv.ts         # Swedish translations
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

The project uses Prettier for code formatting. Configuration is in `.prettierrc`:

```json
{
    "semi": true,
    "tabWidth": 4,
    "printWidth": 100,
    "singleQuote": true,
    "trailingComma": "es5"
}
```

### Adding New Features

1. Create new components in the appropriate directory under `src/components/`
2. Add translations for new features in:
   - `src/i18n/fi.ts` (Finnish)
   - `src/i18n/sv.ts` (Swedish)
3. Update the main `App.tsx` if needed
4. Add any new routes to the React Admin configuration

### Internationalization

To add new translations:

1. Add translation keys and values in the language files
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