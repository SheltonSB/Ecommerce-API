import { useState } from 'react';
import { useLocation, useNavigate } from 'react-router-dom';
import { Button } from '../components/ui/Button';
import { Card } from '../components/ui/Card';
import api from '../api/axios';
import toast from 'react-hot-toast';

function useQuery() {
  return new URLSearchParams(useLocation().search);
}

export function VerifyEmail() {
  const query = useQuery();
  const userIdParam = query.get('userId') || '';
  const tokenParam = query.get('token') || '';
  const [userId, setUserId] = useState(userIdParam);
  const [token, setToken] = useState(tokenParam);
  const [loading, setLoading] = useState(false);
  const navigate = useNavigate();

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    try {
      await api.post('/auth/verify-email', { userId, token });
      toast.success('Email confirmed. You can now log in.');
      navigate('/login');
    } catch {
      // handled globally
    } finally {
      setLoading(false);
    }
  };

  return (
    <Card className="mx-auto max-w-md">
      <div className="mb-6">
        <h1 className="text-xl font-semibold text-gray-900">Verify your email</h1>
        <p className="text-sm text-gray-500">Paste the values from your verification link, or open this page from the email.</p>
      </div>
      <form className="space-y-4" onSubmit={handleSubmit}>
        <div className="space-y-2">
          <label className="text-sm font-medium text-gray-800">User ID</label>
          <input
            className="w-full rounded-md border border-gray-200 bg-white px-3 py-2 text-sm focus:border-indigo-500 focus:outline-none focus:ring-2 focus:ring-indigo-500"
            value={userId}
            onChange={(e) => setUserId(e.target.value)}
            required
          />
        </div>
        <div className="space-y-2">
          <label className="text-sm font-medium text-gray-800">Token</label>
          <textarea
            className="w-full rounded-md border border-gray-200 bg-white px-3 py-2 text-sm focus:border-indigo-500 focus:outline-none focus:ring-2 focus:ring-indigo-500"
            rows={3}
            value={token}
            onChange={(e) => setToken(e.target.value)}
            required
          />
        </div>
        <Button type="submit" disabled={loading} className="w-full">
          {loading ? 'Verifying...' : 'Verify email'}
        </Button>
      </form>
    </Card>
  );
}
