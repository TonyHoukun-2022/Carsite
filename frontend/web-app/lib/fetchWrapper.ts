import { getTokenWorkaround } from '@/app/apis/authApi'

const baseUrl = 'http://localhost:6001/'

async function get(url: string) {
  const requestOpts = {
    method: 'GET',
    headers: await getHeaders(),
  }

  const response = await fetch(baseUrl + url, requestOpts)
  return await handleResponse(response)
}

async function post(url: string, body: {}) {
  const requestOpts = {
    method: 'POST',
    headers: await getHeaders(),
    body: JSON.stringify(body),
  }

  const res = await fetch(baseUrl + url, requestOpts)
  return await handleResponse(res)
}

async function put(url: string, body: {}) {
  const requestOpts = {
    method: 'PUT',
    headers: await getHeaders(),
    body: JSON.stringify(body),
  }

  const res = await fetch(baseUrl + url, requestOpts)
  return await handleResponse(res)
}

async function del(url: string) {
  const requestOpts = {
    method: 'DELETE',
    headers: await getHeaders(),
  }

  const res = await fetch(baseUrl + url, requestOpts)
  return await handleResponse(res)
}

async function getHeaders() {
  const token = await getTokenWorkaround()
  const headers = {
    'Content-type': 'application/json',
  } as any

  if (token) {
    headers.Authorization = 'Bearer ' + token.access_token
  }
  return headers
}

async function handleResponse(response: Response) {
  // read the body of an HTTP response as a plain text string.
  const text = await response.text()
  let data
  if (text) {
    try {
      // Attempt to parse the text as JSON
      data = JSON.parse(text)
    } catch (error) {
      // If parsing fails, the text is not JSON
      data = text
    }
  }

  if (!response.ok) {
    const error = {
      status: response.status,
      message: response.statusText,
    }

    console.log(error)
    return { error }
  }

  return data || response.statusText
}

export const fetchWrapper = {
  get,
  post,
  put,
  del,
}
