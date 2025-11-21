import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { Button } from '../components/ui/Button';
import { Card } from '../components/ui/Card';
import { Input } from '../components/ui/Input';
import api from '../api/axios';
import toast from 'react-hot-toast';

export function Register() {
  const [form, setForm] = useState({
    firstName: '',
    lastName: '',
    phoneNumber: '',
    email: '',
    password: ''
  });
  const [loading, setLoading] = useState(false);
  const navigate = useNavigate();

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setForm((prev) => ({ ...prev, [e.target.name]: e.target.value }));
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    try {
      await api.post('/auth/register', form);
      toast.success('Registered. Please check your email to verify.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <Card className="mx-auto max-w-md">
      <div className="mb-6">
        <h1 className="text-xl font-semibold text-gray-900">Create account</h1>
        <p className="text-sm text-gray-500">
          Join LuxeStore to track orders and save your favorites. We’ll send a verification email to activate your account.
        </p>
      </div>
      <form className="space-y-4" onSubmit={handleSubmit}>
        <div className="grid grid-cols-1 gap-4 md:grid-cols-2">
          <div className="space-y-2">
            <label className="text-sm font-medium text-gray-800">First name</label>
            <Input name="firstName" required value={form.firstName} onChange={handleChange} />
          </div>
          <div className="space-y-2">
            <label className="text-sm font-medium text-gray-800">Last name</label>
            <Input name="lastName" required value={form.lastName} onChange={handleChange} />
          </div>
        </div>
        <div className="space-y-2">
          <label className="text-sm font-medium text-gray-800">Phone</label>
          <Input name="phoneNumber" type="tel" placeholder="+1 (555) 123-4567" value={form.phoneNumber} onChange={handleChange} />
        </div>
        <div className="space-y-2">
          <label className="text-sm font-medium text-gray-800">Email</label>
          <Input name="email" type="email" required value={form.email} onChange={handleChange} />
        </div>
        <div className="space-y-2">
          <label className="text-sm font-medium text-gray-800">Password</label>
          <Input name="password" type="password" required value={form.password} onChange={handleChange} />
        </div>
        <Button type="submit" disabled={loading} className="w-full">
          {loading ? 'Creating account...' : 'Create account'}
        </Button>
      </form>
    </Card>
  );
}
