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

const productSchema = z.object({
  name: z.string().min(1, 'Name is required'),
  description: z.string().min(1, 'Description is required'),
  price: z.number().min(0.01, 'Price must be positive'),
  stockQuantity: z.number().int().min(0, 'Stock cannot be negative'),
  categoryId: z.number().int(),
  imageUrl: z.string().url('A valid image URL is required').optional(),
});

type ProductFormValues = z.infer<typeof productSchema>;

// Mock API functions - replace with actual API calls
const getProducts = async () => {
  // const response = await fetch(`${import.meta.env.VITE_API_URL}/api/products`);
  // return response.json();
  return Promise.resolve([]); // Returning empty for now
};

const createProduct = async (productData: ProductFormValues) => {
  const response = await fetch(`${import.meta.env.VITE_API_URL}/api/admin/products`, {
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

const AdminProducts = () => {
  const [isModalOpen, setIsModalOpen] = useState(false);
  const queryClient = useQueryClient();

  const { data: products, isLoading } = useQuery({
    queryKey: ['adminProducts'],
    queryFn: getProducts,
  });

  const { control, register, handleSubmit, setValue, watch, formState: { errors } } = useForm<ProductFormValues>({
    resolver: zodResolver(productSchema),
    defaultValues: {
      categoryId: 1 // Default to a category
    }
  });

  const imageUrl = watch('imageUrl');

  const createMutation = useMutation({
    mutationFn: createProduct,
    onSuccess: () => {
      toast.success('Product created successfully!');
      queryClient.invalidateQueries({ queryKey: ['adminProducts'] });
      setIsModalOpen(false);
    },
    onError: (error) => {
      toast.error(error.message);
    }
  });

  const onSubmit = (data: ProductFormValues) => {
    createMutation.mutate(data);
  };

  return (
    <div>
      <div className="flex justify-between items-center mb-6">
        <h2 className="text-2xl font-bold">Manage Products</h2>
        <Dialog open={isModalOpen} onOpenChange={setIsModalOpen}>
          <DialogTrigger asChild>
            <Button>Create Product</Button>
          </DialogTrigger>
          <DialogContent>
            <DialogHeader>
              <DialogTitle>Add New Product</DialogTitle>
            </DialogHeader>
            <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
              <Controller
                name="imageUrl"
                control={control}
                render={({ field }) => (
                  <ImageUpload value={field.value ?? ''} onChange={field.onChange} />
                )}
              />
              <Input placeholder="Product Name" {...register('name')} />
              {errors.name && <p className="text-red-500 text-sm">{errors.name.message}</p>}
              <Textarea placeholder="Description" {...register('description')} />
              <Input type="number" placeholder="Price" {...register('price', { valueAsNumber: true })} />
              <Input type="number" placeholder="Stock Quantity" {...register('stockQuantity', { valueAsNumber: true })} />
              <Input type="number" placeholder="Category ID" {...register('categoryId', { valueAsNumber: true })} />
              <Button type="submit" disabled={createMutation.isPending}>
                {createMutation.isPending ? 'Saving...' : 'Save Product'}
              </Button>
            </form>
          </DialogContent>
        </Dialog>
      </div>
      {/* Product Table would go here */}
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