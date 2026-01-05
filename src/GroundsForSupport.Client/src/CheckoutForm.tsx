import { PaymentElement, useStripe, useElements } from '@stripe/react-stripe-js';
import { useState } from 'react';

export default function CheckoutForm() {
  const [isSubmitting, setIsSubmitting] = useState<boolean>(false);
  const stripe = useStripe();
  const elements = useElements();
  const isLoading = !stripe || !elements;

  async function handleSubmit(e: React.FormEvent<HTMLFormElement>) {
    setIsSubmitting(true);

    try {
      e.preventDefault();

      if (!stripe || !elements) {
        return;
      }

      const { error } = await stripe.confirmPayment({
        elements,
        confirmParams: {
          return_url: window.location.origin,
        },
      });

      console.error(error);
      alert(error?.message || 'An unexpected error occurred.');
    } finally {
      setIsSubmitting(false);
    }
  }

  return (
    <form onSubmit={handleSubmit}>
      <PaymentElement
        id='payment-element'
        options={{ layout: 'accordion' }}
      />
      <button
        disabled={isLoading || isSubmitting}
        id='submit'
      >
        <span id='button-text'>Pay</span>
      </button>
    </form>
  );
}
