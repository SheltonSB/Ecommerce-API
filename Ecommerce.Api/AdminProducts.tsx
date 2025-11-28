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
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { toast } from 'sonner';
import ImageUpload from '@/components/ui/ImageUpload';

/**
 * Defines the validation schema for the product form using Zod.
 * This ensures that the data submitted for a new product matches the expected format and constraints.
 */
const productSchema = z.object({
  name: z.string().min(1, 'Name is required'),
  description: z.string().min(1, 'Description is required'),
  price: z.coerce.number().min(0.01, 'Price must be positive'),
  stockQuantity: z.coerce.number().int().min(0, 'Stock cannot be negative'),
  categoryId: z.coerce.number().int().min(1, 'Category ID is required'),
  imageUrl: z.string().url('A valid image URL is required').optional(),
});

/**
 * Type definition for the product form values, inferred from the Zod schema.
 * This provides strong typing for the form data.
 */
type ProductFormValues = z.infer<typeof productSchema>;

/**
 * Fetches the list of all products from the API.
 * @async
 * @returns {Promise<any[]>} A promise that resolves to an array of products.
 */
const getProducts = async () => {
  const response = await fetch(`${import.meta.env.VITE_API_URL}/api/products`, {
    headers: {
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${localStorage.getItem("authToken")}`,
    },
  });
  if (!response.ok) {
    throw new Error('Failed to fetch products');
  }
  return response.json();
};

/**
 * Sends a request to the API to create a new product.
 * @async
 * @param {ProductFormValues} productData - The data for the new product, conforming to the schema.
 * @returns {Promise<any>} A promise that resolves to the newly created product data.
 * @throws {Error} If the API response is not ok.
 */
const createProduct = async (productData: ProductFormValues) => {
  const response = await fetch(`${import.meta.env.VITE_API_URL}/api/products`, {
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
    resolver: zodResolver(productSchema),
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
    onError: (error) => {
      toast.error(error.message);
    }
  });

  const onSubmit = (data: ProductFormValues) => {
    // Trigger the mutation to create the product.
    createMutation.mutate(data);
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
              <Input placeholder="Product Name" {...register('name')} />
              {errors.name && <p className="text-red-500 text-sm">{errors.name.message}</p>}
              <Textarea placeholder="Description" {...register('description')} />
              {errors.description && <p className="text-red-500 text-sm">{errors.description.message}</p>}
              <Input type="number" placeholder="Price" {...register('price', { valueAsNumber: true })} />
              {errors.price && <p className="text-red-500 text-sm">{errors.price.message}</p>}
              <Input type="number" placeholder="Stock Quantity" {...register('stockQuantity', { valueAsNumber: true })} />
              {errors.stockQuantity && <p className="text-red-500 text-sm">{errors.stockQuantity.message}</p>}
              <Input type="number" placeholder="Category ID" {...register('categoryId', { valueAsNumber: true })} />
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