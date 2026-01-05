import '@/App.css';
import { Elements } from '@stripe/react-stripe-js';
import { loadStripe } from '@stripe/stripe-js';
import { useState } from 'react';
import CheckoutForm from './CheckoutForm';
import PaymentForm from './PaymentForm';
import PaymentConfirmationCard from './PaymentConfirmationCard';
import PreviousPaymentsList from './PreviousPaymentsList';

const stripe = loadStripe(import.meta.env.VITE_STRIPE_API_KEY as string);

function App() {
  const queryParams = new URLSearchParams(window.location.search);
  const clientSecretFromUrl = queryParams.get('payment_intent_client_secret');
  const [secret, setSecret] = useState<string | undefined>(clientSecretFromUrl ?? undefined);
  const [isSubmitting, setIsSubmitting] = useState<boolean>(false);

  async function handleDonationFormSubmit(formData: {
    name: string;
    amount: number;
    message?: string;
    email?: string;
  }) {
    setIsSubmitting(true);

    try {
      const url = new URL('/payments/create-intent', import.meta.url);
      const res = await fetch(url, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({
          name: formData.name,
          amount: formData.amount,
          message: formData.message,
          email: formData.email,
        }),
      });

      if (!res.ok) {
        console.error('Failed to create payment intent');
        alert('An error occurred while creating the payment. Please try again.');
        return;
      }

      const data = await res.json() as { clientSecret: string };
      setSecret(data.clientSecret);
    } catch (err) {
      console.error(err);
      alert('An error occurred while creating the payment. Please try again.');
    } finally {
      setIsSubmitting(false);
    }
  }

  return (
    <>
      <header>
        <img
          src='https://github.com/StevanFreeborn.png'
          alt='Profile picture of Stevan Freeborn'
        />
        <div className='info'>
          <h1>Stevan Freeborn</h1>
          <p>
            I'm a dad of 2 who enjoys drinking coffee, lifting weights, and solving problems with
            code. If you have found my open helpful, consider supporting me with a donation which I
            will more than likely spend on more coffee!
          </p>
        </div>
      </header>
      <main>
        <section className='payment'>
          {secret === undefined ? (
            <PaymentForm
              onValidSubmit={handleDonationFormSubmit}
              isSubmitting={isSubmitting}
            />
          ) : (
            <Elements
              options={{
                clientSecret: secret,
                loader: 'auto',
                appearance: {
                  theme: 'stripe',
                  disableAnimations: true,
                  variables: {
                    colorBackground: '#181818',
                    colorPrimary: '#E4E4E4',
                    colorText: '#E4E4E4',
                    fontFamily: 'CaskaydiaCove NFM, monospace',
                    fontSizeBase: '16px',
                    borderRadius: '0.25rem',
                    colorDanger: '#ff6b6b',
                  },
                  rules: {
                    '.Label': {
                      fontWeight: '700',
                    },
                    '.Input': {
                      backgroundColor: '#282828',
                      border: '1px solid #444',
                      padding: '0.5rem',
                    },
                  },
                },
              }}
              stripe={stripe}
            >
              {clientSecretFromUrl ? (
                <PaymentConfirmationCard clientSecret={clientSecretFromUrl} />
              ) : (
                <CheckoutForm />
              )}
            </Elements>
          )}
        </section>
        {clientSecretFromUrl === null ? <PreviousPaymentsList /> : null}
      </main>
    </>
  );
}

export default App;
