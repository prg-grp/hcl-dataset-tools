package utils

func ArrayContains[T comparable](arr []T, pred T) bool {
	for key := range arr {
		if arr[key] == pred {
			return true
		}
	}

	return false
}

func ArrayContainsPred[T comparable](arr []T, pred func(T) bool) bool {
	for key := range arr {
		if pred(arr[key]) {
			return true
		}
	}

	return false
}

func ArrayMap[T any, R any](arr []T, fn func(T) R) []R {
	results := make([]R, 0, len(arr))
	for key, obj := range arr {
		results[key] = fn(obj)
	}

	return results
}
