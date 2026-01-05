import { useState, useEffect } from 'react';
import type { Payment } from './PaymentCard';
import PaymentCard from './PaymentCard';

type PaymentPage = {
  totalNumberOfPayments: number;
  totalNumberOfPages: number;
  currentPageNumber: number;
  payments: Payment[];
};

type PaymentData =
  | {
    status: 'loading';
  }
  | { status: 'error'; message: string }
  | {
    status: 'success';
    pages: PaymentPage[];
  };

export default function PreviousPaymentsList() {
  const [isLoadingMore, setIsLoadingMore] = useState<boolean>(false);
  const [paymentData, setPaymentData] = useState<PaymentData>({ status: 'loading' });
  const payments =
    paymentData.status === 'success' ? paymentData.pages.flatMap(page => page.payments) : [];
  const currentPageNumber =
    paymentData.status === 'success'
      ? paymentData.pages[paymentData.pages.length - 1].currentPageNumber
      : 0;
  const totalNumberOfPages =
    paymentData.status === 'success' ? paymentData.pages[0].totalNumberOfPages : 0;

  useEffect(() => {
    let isMounted = true;

    async function fetchPayments() {
      try {
        const url = new URL('/payments', import.meta.url);
        const res = await fetch(url);

        if (!isMounted) return;

        if (!res.ok) {
          setPaymentData({ status: 'error', message: 'Failed to fetch payments' });
        }

        const data = await res.json();

        setPaymentData({ status: 'success', pages: [data] });
      } catch (err) {
        if (!isMounted) {
          return;
        }

        console.error(err);
        setPaymentData({ status: 'error', message: 'Failed to fetch payments' });
      }
    }

    fetchPayments();

    return () => {
      isMounted = false;
    };
  }, []);

  async function handleLoadMoreButtonClick() {
    if (paymentData.status !== 'success') {
      return;
    }

    setIsLoadingMore(true);

    try {
      const nextPageNumber = currentPageNumber + 1;
      const url = new URL('/payments', import.meta.url);
      const queryParams = new URLSearchParams({ pageNumber: nextPageNumber.toString() });
      const res = await fetch(`${url.toString()}?${queryParams.toString()}`);

      if (!res.ok) {
        console.error('Failed to fetch more payments');
        alert('Failed to load more payments. Please try again.');
        return;
      }

      const data = await res.json();

      setPaymentData({
        status: 'success',
        pages: [...paymentData.pages, data],
      });
    } catch (err) {
      console.error(err);
      alert('Failed to load more payments. Please try again.');
    } finally {
      setIsLoadingMore(false);
    }
  }

  return paymentData.status === 'loading' ? (
    <div>Loading previous donations...</div>
  ) : paymentData.status === 'error' ? (
    <div className='error'>Failed to load previous donations</div>
  ) : (
    <section className='previous-donations'>
      {payments.length == 0 ? null : (
        <>
          <ul>
            {payments.map((payment, index) => (
              <li key={index}>
                <PaymentCard payment={payment} />
              </li>
            ))}
          </ul>
          {currentPageNumber === totalNumberOfPages ? null : (
            <button
              className='load-more-button'
              type='button'
              onClick={handleLoadMoreButtonClick}
              disabled={currentPageNumber >= totalNumberOfPages || isLoadingMore}
            >
              Load More
            </button>
          )}
        </>
      )}
    </section>
  );
}
