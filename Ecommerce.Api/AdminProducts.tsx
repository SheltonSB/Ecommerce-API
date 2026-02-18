/**
 * @file AdminProducts.tsx
 * @author Shelton Bumhe
 * @description This component provides an interface for administrators to manage products.
 * It includes functionality for viewing a list of products and creating new ones through a modal form.
 */
import { useState } from 'react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { Button } from '@/components/ui/button';
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogTrigger } from '@/components/ui/dialog';
import { Input } from '@/components/ui/input';
import { Textarea } from '@/components/ui/textarea';
import { useForm, Controller } from 'react-hook-form';
import { toast } from 'sonner';
import ImageUpload from '@/components/ui/ImageUpload';
const apiBaseUrl = import.meta.env.VITE_API_URL || '';

/**
 * Defines the validation schema for the product form using Zod.
 * This ensures that the data submitted for a new product matches the expected format and constraints.
 */
type ProductFormValues = {
  name: string;
  sku: string;
  description: string;
  price: number;
  stockQuantity: number;
  categoryId: number;
  imageUrl?: string;
};

/**
 * Fetches the list of all products from the API.
 * @async
 * @returns {Promise<any[]>} A promise that resolves to an array of products.
 */
const getProducts = async () => {
  const response = await fetch(`${apiBaseUrl}/api/products`, {
    headers: {
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${localStorage.getItem("authToken")}`,
    },
  });
  if (!response.ok) {
    throw new Error('Failed to fetch products');
  }
  const payload = await response.json();
  return Array.isArray(payload) ? payload : (payload.items ?? []);
};

/**
 * Sends a request to the API to create a new product.
 * @async
 * @param {ProductFormValues} productData - The data for the new product, conforming to the schema.
 * @returns {Promise<any>} A promise that resolves to the newly created product data.
 * @throws {Error} If the API response is not ok.
 */
const createProduct = async (productData: ProductFormValues) => {
  const response = await fetch(`${apiBaseUrl}/api/products`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${localStorage.getItem("authToken")}`,
    },
    body: JSON.stringify(productData),
  });
  if (!response.ok) throw new Error('Failed to create product');
  return response.json();
};

/**
 * The main component for the admin products page.
 * It handles fetching products, displaying them, and provides a dialog
 * for creating new products with form validation.
 * @component
 */
const AdminProducts = () => {
  // State to control the visibility of the "Create Product" modal.
  const [isModalOpen, setIsModalOpen] = useState(false);
  const queryClient = useQueryClient();

  // React Query hook to fetch product data.
  const { data: products, isLoading } = useQuery({
    queryKey: ['adminProducts'],
    queryFn: getProducts,
  });

  // React Hook Form setup for managing the product creation form.
  // It uses Zod for validation and sets a default categoryId.
  const { control, register, handleSubmit, formState: { errors }, reset } = useForm<ProductFormValues>({
    defaultValues: {
      categoryId: 1 // Default to a category
    }
  });

  const createMutation = useMutation({
    mutationFn: createProduct,
    onSuccess: () => {
      toast.success('Product created successfully!');
      // Invalidate the query to refetch the product list with the new item.
      queryClient.invalidateQueries({ queryKey: ['adminProducts'] });
      setIsModalOpen(false);
      reset(); // Reset the form fields after successful submission.
    },
    onError: (error: unknown) => {
      toast.error(error instanceof Error ? error.message : 'Failed to create product');
    }
  });

  const onSubmit = (data: ProductFormValues) => {
    // Trigger the mutation to create the product.
    createMutation.mutate({
      ...data,
      imageUrl: data.imageUrl?.trim() || undefined,
    });
  };

  return (
    <div>
      <div className="flex justify-between items-center mb-6">
        <h2 className="text-2xl font-bold">Manage Products</h2>
        {/* Dialog component for the product creation form */}
        <Dialog open={isModalOpen} onOpenChange={setIsModalOpen}>
          <DialogTrigger asChild>
            <Button>Create Product</Button>
          </DialogTrigger>
          <DialogContent>
            <DialogHeader>
              <DialogTitle>Add New Product</DialogTitle>
            </DialogHeader>
            <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
              {/* Controller for integrating the custom ImageUpload component with React Hook Form */}
              <Controller
                name="imageUrl"
                control={control}
                render={({ field }) => (
                  <ImageUpload 
                    value={field.value ?? ''} 
                    onChange={(url) => field.onChange(url)} />
                )}
              />
              {/* Form fields with validation messages */}
              <Input placeholder="Product Name" {...register('name', { required: 'Name is required' })} />
              {errors.name && <p className="text-red-500 text-sm">{errors.name.message}</p>}
              <Input
                placeholder="SKU"
                {...register('sku', {
                  required: 'SKU is required',
                  minLength: { value: 3, message: 'SKU must be at least 3 characters' }
                })}
              />
              {errors.sku && <p className="text-red-500 text-sm">{errors.sku.message}</p>}
              <Textarea placeholder="Description" {...register('description', { required: 'Description is required' })} />
              {errors.description && <p className="text-red-500 text-sm">{errors.description.message}</p>}
              <Input
                type="number"
                placeholder="Price"
                {...register('price', {
                  valueAsNumber: true,
                  required: 'Price is required',
                  min: { value: 0.01, message: 'Price must be positive' }
                })}
              />
              {errors.price && <p className="text-red-500 text-sm">{errors.price.message}</p>}
              <Input
                type="number"
                placeholder="Stock Quantity"
                {...register('stockQuantity', {
                  valueAsNumber: true,
                  required: 'Stock quantity is required',
                  min: { value: 0, message: 'Stock cannot be negative' }
                })}
              />
              {errors.stockQuantity && <p className="text-red-500 text-sm">{errors.stockQuantity.message}</p>}
              <Input
                type="number"
                placeholder="Category ID"
                {...register('categoryId', {
                  valueAsNumber: true,
                  required: 'Category ID is required',
                  min: { value: 1, message: 'Category ID is required' }
                })}
              />
              {errors.categoryId && <p className="text-red-500 text-sm">{errors.categoryId.message}</p>}
              {errors.imageUrl && <p className="text-red-500 text-sm">{errors.imageUrl.message}</p>}
              <Button type="submit" disabled={createMutation.isPending}>
                {createMutation.isPending ? 'Saving...' : 'Save Product'}
              </Button>
            </form>
          </DialogContent>
        </Dialog>
      </div>
      {/* Display loading state or the list of products */}
      {isLoading ? <p>Loading products...</p> : (
        <div>
          {/* Render products in a table */}
          <p>{products?.length ?? 0} products found.</p>
        </div>
      )}
    </div>
  );
};

export default AdminProducts;
