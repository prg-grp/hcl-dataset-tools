package utils

func MapGetAllKeys[K comparable, T any](m map[K]T) []K {
	keys := make([]K, 0, len(m))
	for key := range m {
		keys = append(keys, key)
	}
	return keys
}

func MapToArray[K comparable, T any, R any](m map[K]T, mapper func(K, T) R) []R {
	res := make([]R, 0, len(m))
	for key, val := range m {
		res = append(res, mapper(key, val))
	}
	return res
}
