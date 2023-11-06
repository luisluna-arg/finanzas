import React, { useEffect, useState } from "react";

function PaginatedHttpRequest({
  url,
  method = "GET",
  headers = {},
  body = null,
  page = 1,
  pageSize = 10,
  children,
}) {
  const [data, setData] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [totalPages, setTotalPages] = useState(0);

  useEffect(() => {
    const fetchData = async () => {
      try {
        const response = await fetch(
          `${url}?Page=${page}&PageSize=${pageSize}`,
          {
            method,
            headers,
            body: body ? JSON.stringify(body) : null,
          }
        );

        if (!response.ok) {
          throw new Error(`HTTP Error: ${response.status}`);
        }

        const result = await response.json();
        setData(result);

        // Calculate the total number of pages
        const totalCount = result.totalItems;

        setTotalPages(Math.ceil(totalCount / pageSize));
      } catch (err) {
        setError(err);
      } finally {
        setLoading(false);
      }
    };

    fetchData();
  }, [url, method, headers, body, page, pageSize]);

  if (loading) {
    // Render loading indicator
    return <div>Loading...</div>;
  }

  if (error) {
    // Render an error message
    return <div>Error: {error.message}</div>;
  }

  // Render the child component with data and total pages
  return children(data, totalPages);
}

export default PaginatedHttpRequest;
