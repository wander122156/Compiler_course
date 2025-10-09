// Reverse
#include<iostream>
#include<string>
#include<algorithm>

int main() {
    std::cout<<"Enter a string: ";
    std:: string input;
    std::cin >> input;
    std::reverse(input.begin(), input.end());
    std::cout <<"Reversed string:" << input << std::endl;
}