import { useState, useEffect } from 'react';
import {
    Dialog,
    DialogTitle,
    DialogContent,
    DialogActions,
    Button,
    Box,
    CircularProgress,
} from '@mui/material';
import { useNotify, useTranslate } from 'react-admin';
import { PageSize } from '../../types';
import AceEditor from 'react-ace';

// Import ace editor themes and modes
import 'ace-builds/webpack-resolver';
import 'ace-builds/src-min-noconflict/ext-language_tools';
import 'ace-builds/src-min-noconflict/mode-handlebars';
import 'ace-builds/src-min-noconflict/mode-json';
import 'ace-builds/src-min-noconflict/theme-twilight';
import 'ace-builds/src-min-noconflict/theme-xcode';

interface TemplateTestDialogProps {
    open: boolean;
    onClose: () => void;
    template: { id: number | string; name?: string; content?: string };
}

const generateModelFromTemplate = (template: string): any => {
    const model: any = {};
    const regex = /{{[#/]?([^}]+)}}/g;
    const eachBlocksContent = new Map<string, Set<string>>();
    
    // First pass: collect all properties used within each blocks
    const eachRegex = /{{#each\s+([^}]+)}}([\s\S]*?){{\/each}}/g;
    let eachMatch;
    while ((eachMatch = eachRegex.exec(template)) !== null) {
        const arrayPath = eachMatch[1].trim();
        const blockContent = eachMatch[2];
        const properties = new Set<string>();
        
        let propMatch;
        const propRegex = /{{([^#/][^}]+)}}/g;
        while ((propMatch = propRegex.exec(blockContent)) !== null) {
            properties.add(propMatch[1].trim());
        }
        
        eachBlocksContent.set(arrayPath, properties);
    }

    // Second pass: process all expressions
    let match;
    while ((match = regex.exec(template)) !== null) {
        const path = match[1].trim();
        
        // Skip each/closing tags and helper functions
        if (path.startsWith('each ') || path.startsWith('/') || path.includes(' ') || path === 'each') {
            // Handle date fields referenced in formatDate
            if (path.startsWith('formatDate ')) {
                const parts = path.split(' ');
                if (parts.length > 1) {
                    const datePath = parts[1];
                    if (!getNestedValue(model, datePath)) {
                        setNestedValue(model, datePath, new Date().toISOString());
                    }
                }
            }
            continue;
        }

        // Handle nested paths
        const parts = path.split('.');
        let current = model;
        
        // If this property is part of an each block, skip it
        let isInEachBlock = false;
        for (const [, properties] of Array.from(eachBlocksContent.entries())) {
            if (properties.has(path)) {
                isInEachBlock = true;
                break;
            }
        }
        
        if (!isInEachBlock) {
            for (let i = 0; i < parts.length - 1; i++) {
                const part = parts[i];
                if (!(part in current)) {
                    current[part] = {};
                }
                current = current[part];
            }

            const lastPart = parts[parts.length - 1];
            if (!(lastPart in current)) {
                current[lastPart] = `Example ${lastPart}`;
            }
        }
    }

    // Create arrays for each blocks with example items
    for (const [arrayPath, properties] of Array.from(eachBlocksContent.entries())) {
        const parts = arrayPath.split('.');
        let current = model;
        
        // Create nested structure for array
        for (let i = 0; i < parts.length - 1; i++) {
            const part = parts[i];
            if (!(part in current)) {
                current[part] = {};
            }
            current = current[part];
        }

        const lastPart = parts[parts.length - 1];
        // Create example array with two items using the actual properties
        const item1: any = {};
        const item2: any = {};
        
        for (const prop of Array.from(properties)) {
            item1[prop] = `Example ${prop} 1`;
            item2[prop] = `Example ${prop} 2`;
        }

        current[lastPart] = [item1, item2];
    }

    return model;
};

// Helper functions
const getNestedValue = (obj: any, path: string): any => {
    const parts = path.split('.');
    let current = obj;
    
    for (const part of parts) {
        if (current === undefined || current === null) return undefined;
        current = current[part];
    }
    
    return current;
};

const setNestedValue = (obj: any, path: string, value: any) => {
    const parts = path.split('.');
    let current = obj;
    
    for (let i = 0; i < parts.length - 1; i++) {
        const part = parts[i];
        if (!(part in current)) {
            current[part] = {};
        }
        current = current[part];
    }
    
    current[parts[parts.length - 1]] = value;
};

export const TemplateTestDialog = ({ open, onClose, template }: TemplateTestDialogProps) => {
    const [model, setModel] = useState('{}');
    const [loading, setLoading] = useState(false);
    const notify = useNotify();
    const translate = useTranslate();
    const theme = localStorage.getItem('theme') || 'light';

    useEffect(() => {
        if (template.content) {
            try {
                const generatedModel = generateModelFromTemplate(template.content);
                setModel(JSON.stringify(generatedModel, null, 2));
            } catch (error) {
                console.error('Error generating model:', error);
                notify('Error generating model from template', { type: 'error' });
            }
        }
    }, [template.content, notify]);

    const handleTest = async () => {
        try {
            setLoading(true);
            let parsedModel;
            try {
                parsedModel = JSON.parse(model);
            } catch (e) {
                notify('templates.test.invalidJson', { type: 'error' });
                return;
            }

            const response = await fetch(`${process.env.REACT_APP_API_URL}/pdf/generate/template`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({
                    templateId: template.id,
                    model: parsedModel,
                    options: {
                        sanitize: true,
                        pageSize: PageSize.A4
                    }
                }),
            });

            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }

            const filename = `${template.name || 'document'}.pdf`;

            // Create a blob from the PDF data
            const blob = await response.blob();
            
            // Create a link element and trigger the download
            const url = window.URL.createObjectURL(blob);
            const a = document.createElement('a');
            a.href = url;
            a.download = filename;
            document.body.appendChild(a);
            a.click();
            window.URL.revokeObjectURL(url);
            document.body.removeChild(a);

            notify('PDF generated successfully', { type: 'success' });
            onClose();
        } catch (error) {
            console.error('Error generating PDF:', error);
            notify('Error generating PDF', { type: 'error' });
        } finally {
            setLoading(false);
        }
    };

    return (
        <Dialog 
            open={open} 
            onClose={onClose} 
            maxWidth={false}
            PaperProps={{
                sx: {
                    width: '90vw',
                    maxWidth: '1800px'
                }
            }}
        >
            <DialogTitle>{translate('templates.test.title', { name: template.name })}</DialogTitle>
            <DialogContent>
                <Box display="flex" gap={4} sx={{ mt: 2 }}>
                    <Box flex={1}>
                        <Box mb={1} fontWeight="bold" color="rgba(0, 0, 0, 0.6)" fontSize="0.75em">
                            {translate('templates.test.template')}
                        </Box>
                        <AceEditor
                            mode="handlebars"
                            theme={theme === 'dark' ? 'twilight' : 'xcode'}
                            name="template-preview"
                            width="100%"
                            height="600px"
                            fontSize={14}
                            showPrintMargin={false}
                            showGutter={true}
                            highlightActiveLine={true}
                            value={template.content || ''}
                            readOnly={true}
                            setOptions={{
                                showLineNumbers: true,
                                tabSize: 4,
                                useWorker: false
                            }}
                        />
                    </Box>
                    <Box flex={1}>
                        <Box mb={1} fontWeight="bold" color="rgba(0, 0, 0, 0.6)" fontSize="0.75em">
                            {translate('templates.test.model')}
                        </Box>
                        <AceEditor
                            mode="json"
                            theme={theme === 'dark' ? 'twilight' : 'xcode'}
                            name="model-editor"
                            width="100%"
                            height="600px"
                            fontSize={14}
                            showPrintMargin={false}
                            showGutter={true}
                            highlightActiveLine={true}
                            value={model}
                            onChange={setModel}
                            setOptions={{
                                enableBasicAutocompletion: true,
                                enableLiveAutocompletion: true,
                                enableSnippets: true,
                                showLineNumbers: true,
                                tabSize: 2,
                                useWorker: false
                            }}
                        />
                    </Box>
                </Box>
            </DialogContent>
            <DialogActions>
                <Button onClick={onClose} disabled={loading}>
                    {translate('ra.action.cancel')}
                </Button>
                <Button
                    onClick={handleTest}
                    variant="contained"
                    color="primary"
                    disabled={loading || !model}
                >
                    {loading ? <CircularProgress size={24} /> : translate('templates.test.download')}
                </Button>
            </DialogActions>
        </Dialog>
    );
}; 