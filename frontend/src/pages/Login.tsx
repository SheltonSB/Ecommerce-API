import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { Button } from '../components/ui/Button';
import { Card } from '../components/ui/Card';
import { Input } from '../components/ui/Input';
import api from '../api/axios';
import toast from 'react-hot-toast';

export function Login() {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [loading, setLoading] = useState(false);
  const navigate = useNavigate();

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    try {
      const response = await api.post('/auth/login', { email, password });
      const token = response.data?.token;
      if (token) {
        localStorage.setItem('token', token);
      }
      toast.success('Welcome back');
      navigate('/home');
    } catch (err: any) {
      const message = err?.response?.data?.message;
      if (message?.toLowerCase().includes('not verified')) {
        toast.error('Please verify your email. Check your inbox for the confirmation link.');
        navigate('/verify-email');
      }
    } finally {
      setLoading(false);
    }
  };

  return (
    <Card className="mx-auto max-w-md">
      <div className="mb-6">
        <h1 className="text-xl font-semibold text-gray-900">Log in</h1>
        <p className="text-sm text-gray-500">
          Access your account to manage orders and cart. Email verification is required before sign in.
        </p>
      </div>
      <form className="space-y-4" onSubmit={handleSubmit}>
        <div className="space-y-2">
          <label className="text-sm font-medium text-gray-800">Email</label>
          <Input type="email" required value={email} onChange={(e) => setEmail(e.target.value)} />
        </div>
        <div className="space-y-2">
          <label className="text-sm font-medium text-gray-800">Password</label>
          <Input type="password" required value={password} onChange={(e) => setPassword(e.target.value)} />
        </div>
        <Button type="submit" disabled={loading} className="w-full">
          {loading ? 'Signing in...' : 'Sign in'}
        </Button>
      </form>
    </Card>
  );
}
