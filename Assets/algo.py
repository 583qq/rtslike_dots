
def binary_search(sorted_list, val):
    low = 0
    high = len(sorted_list) - 1

    while low <= high:
        mid = (low + high) // 2
        guess = sorted_list[mid]
        
        if guess == val:
            return mid
        
        if guess < val:
            low = mid + 1
        else:
            high = mid - 1
    
    return None


def rec_binary(sorted_list, val, _min, _max):
    mid = (_min + _max) // 2
    
    if sorted_list[mid] > val:
        return rec_binary(sorted_list, val, _min, mid - 1)

    if sorted_list[mid] < val:
        return rec_binary(sorted_list, val, mid + 1, _max)
        
    if sorted_list[mid] == val:
        return mid
    
    return None




def countdown(i):
    print(i)
    if i <= 0:
        return
    else:
        countdown(i-1)
    


def rec(arr):
    if len(arr) == 0:
        return 0
    t = arr[0]
    arr.pop(0)
    return t + rec(arr)


def rec_length(arr, current_index):
    if current_index >= len(arr):
        return 0
    return 1 + rec_length(arr, current_index + 1)


def rec_index_search(arr, val, index):
    if(index >= len(arr)):
        return None

    if(arr[index] == val):
        return index

    return rec_index_search(arr, val, index + 1)
    



def rec_max(arr, max):
    if len(arr) == 0:
        return max
    if arr[0] > max:
        max = arr[0]
    arr.pop(0)
    return rec_max(arr, max)


print(rec([10, 15, 15]))

print(rec_max([-6, -17, -20], -20))


arr = [1, 2, 3, 4, 5, 6, 10, 17]

print("Binary Search:", binary_search(arr, 10))
print("Binary Rec Search:", rec_binary(arr, 17, 0, len(arr) - 1))

print("Rec Linear Length: ", rec_length(arr, 0))
print("Rec Linear Index Search: ", rec_index_search(arr, 10, 0))


def quicksort(arr):
    if len(arr) < 2:
        return arr
    
    pivot = arr[0]

    less = [i for i in arr if i < pivot]
    mid = [i for i in arr if i == pivot]
    greater = [i for i in arr if i > pivot]

    return quicksort(less) + mid + quicksort(greater)


print(quicksort([10, 5, 2, 3, -1, 16, 54, 32]))