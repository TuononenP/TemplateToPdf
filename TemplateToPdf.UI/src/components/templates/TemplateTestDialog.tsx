import { useState } from 'react';
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

const simpleModel = {
    title: "Example Title",
    content: "Example content for testing the template."
};

const invoiceModel = {
    company: {
        name: "ACME Corp"
    },
    invoiceNumber: "INV-2024-001",
    customer: {
        name: "John Doe"
    },
    date: new Date().toISOString(),
    items: [
        {
            name: "Widget A",
            quantity: 2,
            price: 19.99
        },
        {
            name: "Widget B",
            quantity: 1,
            price: 29.99
        }
    ],
    total: 69.97
};

export const TemplateTestDialog = ({ open, onClose, template }: TemplateTestDialogProps) => {
    const [model, setModel] = useState(() => {
        const initialModel = template.id === 1 ? invoiceModel : simpleModel;
        return JSON.stringify(initialModel, null, 2);
    });
    const [loading, setLoading] = useState(false);
    const notify = useNotify();
    const translate = useTranslate();
    const theme = localStorage.getItem('theme') || 'light';

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
                    {loading ? <CircularProgress size={24} /> : translate('templates.test.action')}
                </Button>
            </DialogActions>
        </Dialog>
    );
}; 