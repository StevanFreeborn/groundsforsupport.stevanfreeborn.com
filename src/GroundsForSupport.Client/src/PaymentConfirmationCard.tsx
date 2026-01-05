import { useStripe } from '@stripe/react-stripe-js';
import type { PaymentIntent } from '@stripe/stripe-js';
import { useEffect, useState } from 'react';

type PaymentConfirmationCardProps = {
  clientSecret: string;
};

type PaymentIntentData =
  | {
    status: 'loading';
  }
  | {
    status: 'loaded';
    intent: PaymentIntent;
  }
  | {
    status: 'error';
    error: string;
  };

export default function PaymentConfirmationCard({ clientSecret }: PaymentConfirmationCardProps) {
  const stripe = useStripe();

  const [data, setData] = useState<PaymentIntentData>({ status: 'loading' });

  useEffect(() => {
    let isMounted = true;

    async function fetchPaymentIntent() {
      if (!stripe) {
        return;
      }

      try {
        const { paymentIntent, error } = await stripe.retrievePaymentIntent(clientSecret);

        if (!isMounted) return;

        if (error) {
          setData({ status: 'error', error: error.message ?? 'Unknown error' });
          return;
        }

        if (paymentIntent) {
          setData({ status: 'loaded', intent: paymentIntent });
          return;
        }

        setData({ status: 'error', error: 'PaymentIntent not found' });
      } catch (err) {
        if (!isMounted) {
          return;
        }

        console.error(err);
        setData({ status: 'error', error: (err as Error).message });
      }
    }

    fetchPaymentIntent();

    return () => {
      isMounted = false;
    };
  }, [stripe, clientSecret]);

  if (data.status === 'loading') {
    return (
      <div className='payment-confirmation-loading'>
        <div className='spinner' />
        <span>Checking payment status...</span>
      </div>
    );
  }

  return (
    <div className='payment-confirmation-card'>
      {data.status === 'error' ? (
        <>
          <h2>Payment Error</h2>
          <p>There was an error processing your payment: {data.error}</p>
        </>
      ) : (
        <>
          <h2>Thank you!</h2>
          <p>Your payment was successful. I appreciate your support!</p>
        </>
      )}
      <div>
        <a
          className='return-home-link'
          href='/'
        >
          Return to Home
        </a>
      </div>
    </div>
  );
}
