// Tu Factura XML - Main JavaScript File
// Funcionalidades para procesamiento de facturas de proveedores

(function() {
    'use strict';

    // Utility functions
    const Utils = {
        // Show notification
        showNotification: function(message, type = 'info', duration = 3000) {
            const notification = document.createElement('div');
            notification.className = `alert alert-${type} alert-dismissible fade show position-fixed`;
            notification.style.cssText = 'top: 20px; right: 20px; z-index: 9999; min-width: 300px;';
            notification.innerHTML = `
                <i class="fas fa-${type === 'success' ? 'check-circle' : type === 'error' ? 'exclamation-triangle' : 'info-circle'} me-2"></i>
                ${message}
                <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
            `;
            
            document.body.appendChild(notification);
            
            setTimeout(() => {
                if (notification.parentNode) {
                    notification.remove();
                }
            }, duration);
        },

        // Format file size
        formatFileSize: function(bytes) {
            if (bytes === 0) return '0 Bytes';
            const k = 1024;
            const sizes = ['Bytes', 'KB', 'MB', 'GB'];
            const i = Math.floor(Math.log(bytes) / Math.log(k));
            return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + ' ' + sizes[i];
        },

        // Validate file type
        validateFileType: function(file) {
            const allowedTypes = ['.xml', '.zip'];
            const fileName = file.name.toLowerCase();
            return allowedTypes.some(type => fileName.endsWith(type));
        },

        // Add loading state to button
        setButtonLoading: function(button, isLoading, loadingText = 'Procesando...') {
            if (isLoading) {
                button.disabled = true;
                button.dataset.originalText = button.innerHTML;
                button.innerHTML = `<span class="loading me-2"></span>${loadingText}`;
            } else {
                button.disabled = false;
                button.innerHTML = button.dataset.originalText || button.innerHTML;
            }
        }
    };

    // File Upload Handler
    class FileUploadHandler {
        constructor() {
            this.uploadArea = document.getElementById('uploadArea');
            this.fileInput = document.getElementById('fileInput');
            this.fileList = document.getElementById('fileList');
            this.submitBtn = document.getElementById('submitBtn');
            
            if (this.uploadArea && this.fileInput) {
                this.initialize();
            }
        }

        initialize() {
            this.setupDragAndDrop();
            this.setupFileInput();
            this.setupFormSubmission();
        }

        setupDragAndDrop() {
            this.uploadArea.addEventListener('dragover', (e) => {
                e.preventDefault();
                this.uploadArea.classList.add('dragover');
            });

            this.uploadArea.addEventListener('dragleave', () => {
                this.uploadArea.classList.remove('dragover');
            });

            this.uploadArea.addEventListener('drop', (e) => {
                e.preventDefault();
                this.uploadArea.classList.remove('dragover');
                this.handleFiles(e.dataTransfer.files);
            });
        }

        setupFileInput() {
            this.fileInput.addEventListener('change', (e) => {
                this.handleFiles(e.target.files);
            });
        }

        setupFormSubmission() {
            const form = document.getElementById('uploadForm');
            if (form) {
                form.addEventListener('submit', (e) => {
                    if (!this.validateFiles()) {
                        e.preventDefault();
                        Utils.showNotification('Por favor selecciona al menos un archivo válido (.xml o .zip)', 'error');
                        return;
                    }
                    
                    Utils.setButtonLoading(this.submitBtn, true);
                    Utils.showNotification('Procesando facturas de proveedores...', 'info');
                });
            }
        }

        handleFiles(files) {
            this.fileList.innerHTML = '';
            let validFiles = 0;
            
            for (let i = 0; i < files.length; i++) {
                const file = files[i];
                
                if (Utils.validateFileType(file)) {
                    validFiles++;
                    this.addFileToList(file);
                } else {
                    Utils.showNotification(`Archivo "${file.name}" no es válido. Solo se permiten archivos .xml y .zip`, 'error');
                }
            }
            
            this.submitBtn.disabled = validFiles === 0;
            
            if (validFiles > 0) {
                Utils.showNotification(`${validFiles} archivo(s) válido(s) seleccionado(s)`, 'success');
            }
        }

        addFileToList(file) {
            const fileItem = document.createElement('div');
            fileItem.className = 'alert alert-info d-flex align-items-center';
            fileItem.innerHTML = `
                <i class="fas fa-file me-2"></i>
                <span class="flex-grow-1">${file.name} (${Utils.formatFileSize(file.size)})</span>
                <span class="badge badge-success">${file.type || 'Archivo'}</span>
            `;
            this.fileList.appendChild(fileItem);
        }

        validateFiles() {
            const files = this.fileInput.files;
            if (files.length === 0) return false;
            
            for (let i = 0; i < files.length; i++) {
                if (!Utils.validateFileType(files[i])) {
                    return false;
                }
            }
            return true;
        }
    }

    // Initialize when DOM is ready
    document.addEventListener('DOMContentLoaded', function() {
        // Initialize file upload handler
        new FileUploadHandler();

        // Add global error handling
        window.addEventListener('error', function(e) {
            console.error('Global error:', e.error);
            Utils.showNotification('Ha ocurrido un error inesperado', 'error');
        });

        // Add keyboard shortcuts
        document.addEventListener('keydown', function(e) {
            // Ctrl/Cmd + Enter to submit form
            if ((e.ctrlKey || e.metaKey) && e.key === 'Enter') {
                const submitBtn = document.getElementById('submitBtn');
                if (submitBtn && !submitBtn.disabled) {
                    submitBtn.click();
                }
            }
        });
    });

    // Export for global access
    window.TuFacturaXML = {
        Utils: Utils,
        FileUploadHandler: FileUploadHandler
    };

})();
