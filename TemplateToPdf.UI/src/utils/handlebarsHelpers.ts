import Handlebars from 'handlebars';
import { AssetType } from '../types';

// Cache for assets
let imageAssets: any[] = [];
let cssAssets: any[] = [];
let fontAssets: any[] = [];
let assetsLoaded = false;

const fetchAssets = async () => {
    try {
        // Fetch images
        const imageResponse = await fetch(`${process.env.REACT_APP_API_URL}/assets/type/${AssetType.Image}`);
        if (imageResponse.ok) {
            imageAssets = await imageResponse.json();
            console.debug(`Loaded ${imageAssets.length} images`);
        }

        // Fetch CSS
        const cssResponse = await fetch(`${process.env.REACT_APP_API_URL}/assets/type/${AssetType.Css}`);
        if (cssResponse.ok) {
            cssAssets = await cssResponse.json();
            console.debug(`Loaded ${cssAssets.length} CSS assets`);
        }

        // Fetch fonts
        const fontResponse = await fetch(`${process.env.REACT_APP_API_URL}/assets/type/${AssetType.Font}`);
        if (fontResponse.ok) {
            fontAssets = await fontResponse.json();
            console.debug(`Loaded ${fontAssets.length} fonts`);
        }

        assetsLoaded = true;
    } catch (error) {
        console.error('Error fetching assets:', error);
        throw error;
    }
};

export const registerPartialTemplates = async () => {
    try {
        const response = await fetch(`${process.env.REACT_APP_API_URL}/assets/type/${AssetType.PartialTemplate}`);
        if (!response.ok) {
            throw new Error('Failed to fetch partial templates');
        }
        const partials = await response.json();
        
        // Clear existing partials
        for (const name in Handlebars.partials) {
            if (Object.prototype.hasOwnProperty.call(Handlebars.partials, name)) {
                Handlebars.unregisterPartial(name);
            }
        }
        
        partials.forEach((partial: any) => {
            if (partial.content) {
                try {
                    // Pre-compile the partial template to catch any syntax errors early
                    const compiledPartial = Handlebars.compile(partial.content);
                    Handlebars.registerPartial(partial.referenceName, compiledPartial);
                    console.debug(`Registered partial template: ${partial.referenceName}`);
                } catch (error) {
                    console.error(`Failed to compile partial template ${partial.referenceName}:`, error);
                }
            } else {
                console.warn(`Partial template ${partial.referenceName} has no content`);
            }
        });
    } catch (error) {
        console.error('Error registering partial templates:', error);
        throw error;
    }
};

export const registerHandlebarsHelpers = async () => {
    // Ensure assets are loaded
    if (!assetsLoaded) {
        await fetchAssets();
    }

    // Register partial templates
    await registerPartialTemplates();

    // Asset helpers
    Handlebars.registerHelper('image', function(referenceName) {
        if (!referenceName) {
            console.warn('No reference name provided for image helper');
            return '';
        }
        const image = imageAssets.find(img => img.referenceName === referenceName);
        if (!image || !image.binaryContent) {
            console.warn(`Image not found: ${referenceName}`);
            return '';
        }
        return new Handlebars.SafeString(`<img src="data:${image.mimeType};base64,${image.binaryContent}" alt="${image.name}" />`);
    });

    Handlebars.registerHelper('css', function(referenceName) {
        if (!referenceName) {
            console.warn('No reference name provided for css helper');
            return '';
        }
        const css = cssAssets.find(c => c.referenceName === referenceName);
        if (!css || !css.content) {
            console.warn(`CSS not found: ${referenceName}`);
            return '';
        }
        return new Handlebars.SafeString(`<style>${css.content}</style>`);
    });

    Handlebars.registerHelper('font', function(referenceName) {
        if (!referenceName) {
            console.warn('No reference name provided for font helper');
            return '';
        }
        const font = fontAssets.find(f => f.referenceName === referenceName);
        if (!font || !font.binaryContent) {
            console.warn(`Font not found: ${referenceName}`);
            return '';
        }
        return new Handlebars.SafeString(`
            <style>
                @font-face {
                    font-family: '${font.name}';
                    src: url(data:font/woff2;base64,${font.binaryContent}) format('woff2');
                }
            </style>
        `);
    });

    // String helpers
    Handlebars.registerHelper('uppercase', function(str) {
        return (str || '').toString().toUpperCase();
    });

    Handlebars.registerHelper('lowercase', function(str) {
        return (str || '').toString().toLowerCase();
    });

    Handlebars.registerHelper('substring', function(str, start, length) {
        str = (str || '').toString();
        start = parseInt(start || '0');
        length = length ? parseInt(length) : str.length - start;
        return str.substr(start, Math.min(length, str.length - start));
    });

    // Number helpers
    Handlebars.registerHelper('formatNumber', function(number, format) {
        if (!number) return '0';
        const num = parseFloat(number);
        if (isNaN(num)) return '0';
        
        // Basic format implementation
        const decimals = format === 'N0' ? 0 : format === 'N1' ? 1 : 2;
        return num.toFixed(decimals);
    });

    Handlebars.registerHelper('formatCurrency', function(number, locale = 'en-US') {
        if (!number) return '$0.00';
        const num = parseFloat(number);
        if (isNaN(num)) return '$0.00';
        
        return new Intl.NumberFormat(locale, {
            style: 'currency',
            currency: 'USD'
        }).format(num);
    });

    // Math helpers
    Handlebars.registerHelper('sum', function(...args) {
        const numbers = args.slice(0, -1); // Remove the last argument (Handlebars options)
        return numbers.reduce((sum, num) => {
            const parsed = parseFloat(num);
            return sum + (isNaN(parsed) ? 0 : parsed);
        }, 0);
    });

    Handlebars.registerHelper('multiply', function(a, b) {
        return (parseFloat(a) || 0) * (parseFloat(b) || 0);
    });

    Handlebars.registerHelper('divide', function(a, b) {
        const divisor = parseFloat(b) || 0;
        if (divisor === 0) return 0;
        return (parseFloat(a) || 0) / divisor;
    });

    Handlebars.registerHelper('round', function(number, decimals = 0) {
        const num = parseFloat(number);
        if (isNaN(num)) return 0;
        return Number(Math.round(parseFloat(num + 'e' + decimals)) + 'e-' + decimals);
    });

    // Array helpers
    Handlebars.registerHelper('length', function(arr) {
        return Array.isArray(arr) ? arr.length : 0;
    });

    Handlebars.registerHelper('join', function(arr, separator = ',') {
        return Array.isArray(arr) ? arr.join(separator) : '';
    });

    // Conditional helpers
    Handlebars.registerHelper('eq', function(a, b) {
        return a === b;
    });

    Handlebars.registerHelper('neq', function(a, b) {
        return a !== b;
    });

    Handlebars.registerHelper('gt', function(a, b) {
        return parseFloat(a) > parseFloat(b);
    });

    Handlebars.registerHelper('lt', function(a, b) {
        return parseFloat(a) < parseFloat(b);
    });

    Handlebars.registerHelper('gte', function(a, b) {
        return parseFloat(a) >= parseFloat(b);
    });

    Handlebars.registerHelper('lte', function(a, b) {
        return parseFloat(a) <= parseFloat(b);
    });

    // Date helpers
    Handlebars.registerHelper('formatDate', function(date: Date, format: string) {
        if (!date) return '';
        try {
            const d = new Date(date);
            return format
                .replace('MM', String(d.getMonth() + 1).padStart(2, '0'))
                .replace('dd', String(d.getDate()).padStart(2, '0'))
                .replace('yyyy', String(d.getFullYear()));
        } catch (error) {
            return '';
        }
    });

    Handlebars.registerHelper('now', function(format = 'yyyy-MM-dd') {
        const d = new Date();
        return format
            .replace('MM', String(d.getMonth() + 1).padStart(2, '0'))
            .replace('dd', String(d.getDate()).padStart(2, '0'))
            .replace('yyyy', String(d.getFullYear()));
    });

    Handlebars.registerHelper('addDays', function(date, days, format = 'yyyy-MM-dd') {
        if (!date || isNaN(days)) return '';
        try {
            const d = new Date(date);
            d.setDate(d.getDate() + parseInt(days));
            return format
                .replace('MM', String(d.getMonth() + 1).padStart(2, '0'))
                .replace('dd', String(d.getDate()).padStart(2, '0'))
                .replace('yyyy', String(d.getFullYear()));
        } catch (error) {
            return '';
        }
    });
}; 