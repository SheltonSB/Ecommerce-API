import { useState } from 'react';
import { useDropzone } from 'react-dropzone';
import { UploadCloud, X } from 'lucide-react';
import { toast } from 'sonner';

interface ImageUploadProps {
  value: string;
  onChange: (url: string) => void;
}

const ImageUpload = ({ value, onChange }: ImageUploadProps) => {
  const [isUploading, setIsUploading] = useState(false);

  const onDrop = async (acceptedFiles: File[]) => {
    const file = acceptedFiles[0];
    if (!file) return;

    setIsUploading(true);
    const formData = new FormData();
    formData.append('file', file);

    try {
      const response = await fetch(`${import.meta.env.VITE_API_URL}/api/admin/photos/upload`, {
        method: 'POST',
        headers: {
          'Authorization': `Bearer ${localStorage.getItem("authToken")}`,
        },
        body: formData,
      });

      if (!response.ok) throw new Error('Upload failed');

      const data = await response.json();
      onChange(data.url);
      toast.success('Image uploaded successfully!');
    } catch (error) {
      toast.error('Failed to upload image.');
    } finally {
      setIsUploading(false);
    }
  };

  const { getRootProps, getInputProps } = useDropzone({ onDrop, accept: { 'image/*': [] } });

  if (value) {
    return (
      <div className="relative">
        <img src={value} alt="Uploaded product" className="w-full h-48 object-cover rounded-md" />
        <button onClick={() => onChange('')} className="absolute top-2 right-2 bg-red-500 text-white p-1 rounded-full">
          <X className="h-4 w-4" />
        </button>
      </div>
    );
  }

  return (
    <div {...getRootProps()} className="border-2 border-dashed border-gray-300 rounded-md p-8 text-center cursor-pointer">
      <input {...getInputProps()} />
      <UploadCloud className="mx-auto h-12 w-12 text-gray-400" />
      <p className="mt-2 text-sm text-gray-600">{isUploading ? 'Uploading...' : 'Drag & drop an image here, or click to select one'}</p>
    </div>
  );
};

export default ImageUpload;